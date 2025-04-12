from .BlissSystemCallback import BlissSystemCallback

from ...CallbackState import CallbackState
from ...DTO import BatchResult


class LossCallback(BlissSystemCallback):
    def on_train_batch_end(self,
                           batch_result: BatchResult,
                           callback_state: CallbackState,
                           *args, **kwargs) -> None:
        for loss_name, loss_value in batch_result.losses.items():
            callback_state.update_loss_values(loss_name, loss_value, train=True)

    def on_eval_batch_end(self,
                          batch_result: BatchResult,
                          callback_state: CallbackState,
                          *args, **kwargs) -> None:
        for loss_name, loss_value in batch_result.losses.items():
            callback_state.update_loss_values(loss_name, loss_value, train=False)

    def on_train_epoch_end(self,
                           callback_state: CallbackState,
                           *args, **kwargs) -> None:
        callback_state.accumulate_train_loss_batch_to_epoch()
        callback_state.clear_train_loss_batch()

    def on_eval_epoch_end(self,
                          callback_state: CallbackState,
                          *args, **kwargs) -> None:
        callback_state.accumulate_eval_loss_batch_to_epoch()
        callback_state.clear_eval_loss_batch()