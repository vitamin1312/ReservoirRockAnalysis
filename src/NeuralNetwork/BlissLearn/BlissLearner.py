import torch
from torch import nn
from torch.utils.data import DataLoader
import sys
try:
    from tqdm.notebook import tqdm
except ImportError:
    from tqdm import tqdm

from .BlissTypes import (criterion,
                        nullable_scheduler,
                        nullable_list,
                        optimizer_type,
                        step_function)


class BlissLearner:
    def __init__(self,
                 model: nn.Module,
                 loss_function: criterion,
                 optimizer_class: optimizer_type,
                 optimizer_kwargs: dict,
                 train_dataloader: DataLoader,
                 test_dataloader: DataLoader,
                 # batch_callbacks: nullable_list = None,
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
        # self.batch_callbacks = batch_callbacks

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
        # with torch.no_grad():
        #     for callback in self.batch_callbacks:
        #         callback.on_batch_end(yb, outputs)
        return loss.item()

    def validate_step(self, xb: torch.Tensor, yb: torch.Tensor) -> float:
        with torch.no_grad():
            loss, outputs = self.calc_loss(xb, yb)

            # for callback in self.batch_callbacks:
            #     callback.on_batch_end(yb, outputs)

        return loss.item()

    ################### Train loop ###################
    def fit(self, number_of_epochs: int = 1) -> None:
        for epoch in range(1, number_of_epochs + 1):
            print(f'epoch: {epoch}', flush=True)
            sys.stdout.flush()
            self.do_epoch(self.train_step)
            self.do_epoch(self.validate_step)

    # https://stackoverflow.com/questions/64727187/tqdm-multiple-progress-bars-with-nested-for-loops-in-pycharm
    def do_epoch(self, batch_processing: step_function) -> None:
        for batch_number, (xb, yb) in tqdm(enumerate(self.train_dataloader),
                                     position=1,
                                     desc="batches",
                                     leave=False,
                                     ncols=500,
                                     total=len(self.train_dataloader)):
            batch_processing(xb, yb)
