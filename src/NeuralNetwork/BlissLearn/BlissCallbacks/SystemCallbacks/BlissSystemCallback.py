from abc import ABC, abstractmethod

from ...CallbackState import CallbackState
from ...DTO import BatchResult


class BlissSystemCallback(ABC):
    @abstractmethod
    def on_train_batch_end(self,
                           batch_result: BatchResult,
                           callback_state: CallbackState,
                           *args, **kwargs) -> None:
        pass

    @abstractmethod
    def on_eval_batch_end(self,
                          batch_result: BatchResult,
                          callback_state: CallbackState,
                          *args, **kwargs) -> None:
        pass

    @abstractmethod
    def on_train_epoch_end(self,
                           callback_state: CallbackState,
                           *args, **kwargs) -> None:
        pass

    @abstractmethod
    def on_eval_epoch_end(self,
                          callback_state: CallbackState,
                          *args, **kwargs) -> None:
        pass