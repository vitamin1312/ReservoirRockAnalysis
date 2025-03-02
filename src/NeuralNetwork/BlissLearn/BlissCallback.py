import torch
from abc import ABC, abstractmethod


class BlissSystemCallback(ABC):
    @abstractmethod
    def on_train_batch_end(self,
                           loss: float,
                           callback_state,
                           *args, **kwargs) -> None:
        pass

    @abstractmethod
    def on_eval_batch_end(self,
                          loss: float,
                          callback_state,
                          *args, **kwargs) -> None:
        pass

    @abstractmethod
    def on_train_epoch_end(self,
                           callback_state,
                           *args, **kwargs) -> None:
        pass

    @abstractmethod
    def on_eval_epoch_end(self,
                          callback_state,
                          *args, **kwargs) -> None:
        pass



class BlissCallback(ABC):
    @abstractmethod
    def on_train_batch_end(self,
                           yb: torch.Tensor,
                           outputs: torch.Tensor,
                           callback_state,
                           *args, **kwargs) -> None:
        pass

    @abstractmethod
    def on_eval_batch_end(self,
                          yb: torch.Tensor,
                          outputs: torch.Tensor,
                          callback_state,
                          *args, **kwargs) -> None:
        pass

    @abstractmethod
    def on_train_epoch_end(self,
                           callback_state,
                           *args, **kwargs) -> None:
        pass

    @abstractmethod
    def on_eval_epoch_end(self,
                          callback_state,
                          *args, **kwargs) -> None:
        pass