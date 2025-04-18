import torch
from torch import nn
from torch.utils.data import DataLoader
import sys
def is_notebook():
    return "ipykernel" in sys.modules
if is_notebook():
    from tqdm.notebook import tqdm
else:
    from tqdm import tqdm

from abc import ABC, abstractmethod

from .CallbackState import CallbackState
from .BlissCallbacks.SystemCallbacks import LossCallback

from .BlissTypes.BlissCommonTypes import nullable_scheduler, optimizer_type
from .BlissTypes.BlissFunctionTypes import criterion
from .BlissTypes.LearnerTypes import (
    nullable_system_callbacks_list,
    nullable_callbacks_list,
    step_function
)

from .DTO import BatchResult, ColorizationBatchResult


class _BaseBlissLearner(ABC):
    def __init__(self,
                 train_dataloader: DataLoader,
                 test_dataloader: DataLoader,
                 callbacks: nullable_callbacks_list = None,
                 batches_to_validate: int | None = None, # Early validating

                 # todo: add next fields using
                 scheduler: nullable_scheduler = None,
                 *args,
                 **kwargs
                 ) -> None:

        self.train_dataloader = train_dataloader
        self.test_dataloader = test_dataloader

        self.callbacks = callbacks or []
        system_callbacks = self.get_system_callbacks()
        self.system_callbacks = system_callbacks or []

        if batches_to_validate is not None and batches_to_validate > 0:
            self.batches_to_validate = batches_to_validate
        else:
            self.batches_to_validate = None

        self.use_early_validation = bool(self.batches_to_validate)


        self._callback_state = CallbackState()

    ################### Train loop ###################
    def fit(self, number_of_epochs: int = 1) -> None:
        for epoch in range(1, number_of_epochs + 1):

            print(f'epoch: {epoch}', flush=True)
            self._do_train_epoch()
            self._do_validate_epoch()

    def _do_train_epoch(self):
        print(f'training', flush=True)
        sys.stdout.flush()
        self._do_epoch(self._full_train_step, self.train_dataloader, train=True)
    
    def _do_validate_epoch(self):
        if not self.use_early_validation:
            print(f'validating', flush=True)
            sys.stdout.flush()
            self._do_epoch(self._full_validate_step, self.test_dataloader, train=False)

    def _validate_on_the_fly(self):
        for xb, yb in tqdm(
                self.test_dataloader,
                position=1,
                desc="batches",
                leave=False,
                ncols=1200,
                total=len(self.test_dataloader)):

            self._full_validate_step(xb, yb)

    def _early_validate(self):
        print('early validation')
        print('train info')
        self._use_epoch_callbacks(train=True)
        print('eval info')
        self._validate_on_the_fly()
        self._use_epoch_callbacks(train=False)

    def _use_epoch_callbacks(self, train=True):
        if train:
            for callback in self.system_callbacks: callback.on_train_epoch_end(self._callback_state)
            for callback in self.callbacks: callback.on_train_epoch_end(self._callback_state)
        else:
            for callback in self.system_callbacks: callback.on_eval_epoch_end(self._callback_state)
            for callback in self.callbacks: callback.on_eval_epoch_end(self._callback_state)

    # https://stackoverflow.com/questions/64727187/tqdm-multiple-progress-bars-with-nested-for-loops-in-pycharm
    def _do_epoch(self, batch_processing: step_function, dataloader: DataLoader, train=True) -> None:
        virtual_batch = 0
        validated = False
        for _, (xb, yb) in tqdm(
                enumerate(dataloader),
                position=1,
                desc="batches",
                leave=False,
                ncols=1200,
                total=len(dataloader)
        ):
            batch_processing(xb, yb)

            if train and self.use_early_validation:
                virtual_batch += 1
                validated = False
                if virtual_batch >= self.batches_to_validate:
                    virtual_batch = 0
                    self._early_validate()
                    validated = True
        if self.use_early_validation and not validated:
            self._early_validate()
        else:
            self._use_epoch_callbacks(train)

    def _full_train_step(self, xb: torch.Tensor, yb: torch.Tensor) -> None:
        batch_result = self.train_step(xb, yb)
        with torch.no_grad():
            for callback in self.system_callbacks: callback.on_train_batch_end(batch_result, self._callback_state)
            for callback in self.callbacks: callback.on_train_batch_end(batch_result, self._callback_state)

    def _full_validate_step(self, xb: torch.Tensor, yb: torch.Tensor) -> None:
        with torch.no_grad():
            batch_result = self.validate_step(xb, yb)
            for callback in self.system_callbacks: callback.on_eval_batch_end(batch_result, self._callback_state)
            for callback in self.callbacks: callback.on_eval_batch_end(batch_result, self._callback_state)

    @abstractmethod
    def train_step(self, xb: torch.Tensor, yb: torch.Tensor) -> BatchResult:
        pass

    @abstractmethod
    def validate_step(self, xb: torch.Tensor, yb: torch.Tensor) -> BatchResult:
        pass

    ################### Class utils ###################
    @staticmethod
    def get_system_callbacks() -> nullable_system_callbacks_list:
        return [LossCallback()]
    
    def get_train_info(self):
        return self._callback_state._get_all_criteria()

class BlissLearner(_BaseBlissLearner):
    def __init__(self,
                 model: nn.Module,
                 loss_function: criterion,
                 optimizer_class: optimizer_type,
                 optimizer_kwargs: dict,
                 train_dataloader: DataLoader,
                 test_dataloader: DataLoader,
                 callbacks: nullable_callbacks_list = None,
                 batches_to_validate: int | None = None,

                 # todo: add next fields using
                 scheduler: nullable_scheduler = None,
                 use_amp: bool = False,

                 *args,
                 **kwargs
                 ) -> None:
        
        super().__init__(
            train_dataloader=train_dataloader,
            test_dataloader=test_dataloader,
            callbacks=callbacks,
            batches_to_validate=batches_to_validate,
            *args,
            **kwargs
        )
        
        self._model = model
        self._loss_function = loss_function
        self._optimizer = optimizer_class(model.parameters(), **optimizer_kwargs)
        self._callback_state = CallbackState()

    def train_step(self, xb: torch.Tensor, yb: torch.Tensor) -> BatchResult:
        self._optimizer.zero_grad()
        self._model.train()
        loss, outputs = self._calc_loss_with_grad(xb, yb)
        self._optimizer.step()
        return BatchResult.from_losses_dict(
            losses=loss,
            yb=yb,
            outputs=outputs
        )

    def validate_step(self, xb: torch.Tensor, yb: torch.Tensor) -> BatchResult:
        self._model.eval()
        loss, outputs = self._calc_loss(xb, yb)
        return BatchResult.from_losses_dict(
            losses=loss,
            yb=yb,
            outputs=outputs
        )

    ################### Loss calculation ###################
    def _calc_loss_function(self, outputs: torch.Tensor, yb: torch.Tensor) -> torch.Tensor:
        return self._loss_function(outputs, yb)

    def _calc_loss_with_grad(self, xb: torch.Tensor, yb: torch.Tensor) -> tuple[torch.Tensor, torch.Tensor]:
        outputs = self._model(xb)
        loss = self._calc_loss_function(outputs, yb)
        loss.backward()
        return loss, outputs

    def _calc_loss(self, xb: torch.Tensor, yb: torch.Tensor) -> tuple[torch.Tensor, torch.Tensor]:
        outputs = self._model(xb)
        loss = self._calc_loss_function(outputs, yb)
        return loss, outputs


class BlissColorizationLearner(_BaseBlissLearner):
    def __init__(self,
                 generator: nn.Module,
                 discriminator: nn.Module,
                 generator_loss_function: criterion,
                 discriminator_loss_function: criterion,
                 generator_optimizer_class: optimizer_type,
                 generator_optimizer_kwargs: dict,
                 discriminator_optimizer_class: optimizer_type,
                 discriminator_optimizer_kwargs: dict,
                 train_dataloader: DataLoader,
                 test_dataloader: DataLoader,
                 alpha: float,
                 callbacks: nullable_callbacks_list = None,
                 batches_to_validate: int | None = None,

                 # todo: add next fields using
                 scheduler: nullable_scheduler = None,
                 use_amp: bool = False,
                 *args,
                 **kwargs
                 ) -> None:

        super().__init__(
            train_dataloader=train_dataloader,
            test_dataloader=test_dataloader,
            callbacks=callbacks,
            batches_to_validate=batches_to_validate,
            *args,
            **kwargs
        )

        self._generator = generator
        self._discriminator = discriminator
        self._generator_loss_function = generator_loss_function
        self._discriminator_loss_function = discriminator_loss_function
        self._generator_optimizer = generator_optimizer_class(
            self._generator.parameters(),
            **generator_optimizer_kwargs
        )
        self._discriminator_optimizer = discriminator_optimizer_class(
            self._discriminator.parameters(),
            **discriminator_optimizer_kwargs
        )
        self._alpha = alpha

    def train_step(self, xb: torch.Tensor, yb: torch.Tensor) -> ColorizationBatchResult:
        # Preprocessing
        self._generator_optimizer.zero_grad()
        self._discriminator_optimizer.zero_grad()
        self._generator.train()
        self._discriminator.train()

        # Train discriminator
        y_fake = self._generator(xb).detach()
        # Fake inputs
        y_fake_preds = self._discriminator(xb, y_fake)
        y_fake_labels = torch.zeros_like(y_fake_preds)
        loss_fake = self._discriminator_loss_function(y_fake_preds, y_fake_labels)
        # True inputs
        y_true_preds = self._discriminator(xb, yb)
        y_true_labels = torch.ones_like(y_true_preds)
        loss_true = self._discriminator_loss_function(y_true_preds, y_true_labels)
        # Loss
        discriminator_loss = loss_true + loss_fake
        discriminator_loss.backward()
        self._discriminator_optimizer.step()

        # Train generator
        y_fake = self._generator(xb)
        y_fake_preds = self._discriminator(xb, y_fake)
        y_fake_labels = torch.ones_like(y_fake_preds)

        # Loss
        fake_loss = self._discriminator_loss_function(y_fake_preds, y_fake_labels)
        generator_loss = fake_loss + self._alpha * self._generator_loss_function(y_fake, yb)
        generator_loss.backward()
        self._generator_optimizer.step()
        torch.cuda.empty_cache()

        return ColorizationBatchResult.from_losses_dict(
            losses={'generator_loss': generator_loss, 'discriminator_loss': discriminator_loss},
            y_true=yb,
            y_fake=y_fake,
            y_true_labels_preds=y_true_preds,
            y_fake_labels_preds=y_fake_preds
        )


    def validate_step(self, xb: torch.Tensor, yb: torch.Tensor) -> ColorizationBatchResult:
        self._generator.eval()
        self._discriminator.eval()

        y_fake = self._generator(xb)

        # Eval discriminator
        # Fake inputs
        y_fake_preds = self._discriminator(xb, y_fake)
        y_fake_labels = torch.zeros_like(y_fake_preds)
        loss_fake = self._discriminator_loss_function(y_fake_preds, y_fake_labels)
        # True inputs
        y_true_preds = self._discriminator(xb, yb)
        y_true_labels = torch.ones_like(y_true_preds)
        loss_true = self._discriminator_loss_function(y_true_preds, y_true_labels)

        discriminator_loss = loss_true + loss_fake

        # Eval generator
        y_fake_preds = self._discriminator(xb, y_fake)
        y_fake_labels = torch.ones_like(y_fake_preds)

        fake_loss = self._discriminator_loss_function(y_fake_preds, y_fake_labels)
        generator_loss = fake_loss + self._alpha * self._generator_loss_function(y_fake, yb)
        torch.cuda.empty_cache()

        return ColorizationBatchResult.from_losses_dict(
            losses={'generator_loss': generator_loss, 'discriminator_loss': discriminator_loss},
            y_true=yb,
            y_fake=y_fake,
            y_true_labels_preds=y_true_preds,
            y_fake_labels_preds=y_fake_preds
        )
