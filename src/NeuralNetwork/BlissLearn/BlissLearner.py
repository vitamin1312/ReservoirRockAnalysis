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

from CallbackState import CallbackState
from SystemBlissCallbacks import LossCallback
from BlissTypes import (criterion,
                        nullable_scheduler,
                        optimizer_type,
                        step_function,
                        nullable_callbacks_list,
                        nullable_system_callbacks_list,
                        nullable_system_callbacks_getter)

def get_system_callbacks() -> nullable_system_callbacks_list:
    return [LossCallback()]


class BlissLearner:
    def __init__(self,
                 model: nn.Module,
                 loss_function: criterion,
                 optimizer_class: optimizer_type,
                 optimizer_kwargs: dict,
                 train_dataloader: DataLoader,
                 test_dataloader: DataLoader,
                 callbacks: nullable_callbacks_list = None,
                 system_callbacks_getter: nullable_system_callbacks_getter = get_system_callbacks,

                 # todo: add next fields using
                 scheduler: nullable_scheduler = None,
                 use_amp: bool = False,
                 *args,
                 **kwargs
                 ):
        
        self.model = model
        self.loss_function = loss_function

        self.train_dataloader = train_dataloader
        self.test_dataloader = test_dataloader

        self.optimizer = optimizer_class(model.parameters(), **optimizer_kwargs)

        self.callbacks = callbacks if callbacks else []
        system_callbacks = system_callbacks_getter()
        self.system_callbacks = system_callbacks if system_callbacks else []

        self.callback_state = CallbackState()

    ################### Loss calculation ###################
    def calc_loss_function(self, outputs: torch.Tensor, yb: torch.Tensor) -> torch.Tensor:
        return self.loss_function(outputs, yb)

    def calc_loss_with_grad(self, xb: torch.Tensor, yb: torch.Tensor) -> tuple[torch.Tensor, torch.Tensor]:
        outputs = self.model(xb)
        loss = self.calc_loss_function(outputs, yb)
        loss.backward()
        return loss, outputs

    def calc_loss(self, xb: torch.Tensor, yb: torch.Tensor) -> tuple[torch.Tensor, torch.Tensor]:
        outputs = self.model(xb)
        loss = self.calc_loss_function(outputs, yb)
        return loss, outputs

    def train_step(self, xb: torch.Tensor, yb: torch.Tensor) -> float:
        self.optimizer.zero_grad()
        loss, outputs = self.calc_loss_with_grad(xb, yb)
        self.optimizer.step()
        with torch.no_grad():
            for callback in self.system_callbacks: callback.on_train_batch_end(loss.item(), self.callback_state)
            for callback in self.callbacks: callback.on_train_batch_end(yb, outputs, self.callback_state)

        return loss.item()

    def validate_step(self, xb: torch.Tensor, yb: torch.Tensor) -> float:
        with torch.no_grad():
            loss, outputs = self.calc_loss(xb, yb)
            for callback in self.system_callbacks: callback.on_eval_batch_end(loss.item(), self.callback_state)
            for callback in self.callbacks: callback.on_eval_batch_end(yb, outputs, self.callback_state)

        return loss.item()

    ################### Train loop ###################
    def fit(self, number_of_epochs: int = 1) -> None:
        for epoch in range(1, number_of_epochs + 1):
            print(f'epoch: {epoch}', flush=True)
            print(f'training', flush=True)
            sys.stdout.flush()

            self.do_epoch(self.train_step, self.train_dataloader)
            for callback in self.system_callbacks: callback.on_train_epoch_end(self.callback_state)
            for callback in self.callbacks: callback.on_train_epoch_end(self.callback_state)

            print(f'validating', flush=True)
            sys.stdout.flush()

            self.do_epoch(self.validate_step, self.test_dataloader)
            for callback in self.system_callbacks: callback.on_eval_epoch_end(self.callback_state)
            for callback in self.callbacks: callback.on_eval_epoch_end(self.callback_state)

    # https://stackoverflow.com/questions/64727187/tqdm-multiple-progress-bars-with-nested-for-loops-in-pycharm
    @staticmethod
    def do_epoch(batch_processing: step_function, dataloader: DataLoader) -> None:
        for batch_number, (xb, yb) in tqdm(
                enumerate(dataloader),
                position=1,
                desc="batches",
                leave=False,
                ncols=80,
                total=len(dataloader)
        ):

            batch_processing(xb, yb)
