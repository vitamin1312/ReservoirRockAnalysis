import sys
from collections import defaultdict
import torch

from BlissCallback import BlissCallback
from BlissTypes import nullable_criterion_dict
from CallbackState import CallbackState

class SegmentationMetricsCallback(BlissCallback):
    def __init__(
            self,
            num_classes: int ,
            common_metrics: nullable_criterion_dict = None,
            class_metrics: nullable_criterion_dict = None
    ) -> None:

        self.num_classes = num_classes
        self.common_metrics = common_metrics if common_metrics else dict()
        self.class_metrics = class_metrics if class_metrics else dict()

    def calculate_common_metrics(self, yb: torch.Tensor, outputs: torch.Tensor) -> dict[str, float]:
        common_metrics_values = dict()
        for name, metric in self.common_metrics.items():
            common_metrics_values[name] = metric(yb, outputs)
        return common_metrics_values

    def calculate_class_metrics(self, yb: torch.Tensor, outputs: torch.Tensor) -> dict[str, float]:
        class_metrics_values = dict()
        for name, metric in self.class_metrics.items():
            for class_num in range(self.num_classes):
                class_outputs = torch.where(outputs == class_num, 1, 0).argmax(axis=1)
                class_targets = torch.where(yb == class_num, 1, 0)
                class_metrics_values[name + f'class{class_num}'] = metric(class_outputs, class_targets)
        return class_metrics_values


    def on_batch_end(self,
                     yb: torch.Tensor,
                     outputs: torch.Tensor
                     ) -> dict[str, float]:
        common_metrics = self.calculate_common_metrics(yb, outputs)
        class_metrics = self.calculate_class_metrics(yb, outputs)

        return common_metrics | class_metrics

    def on_train_batch_end(self,
                           yb: torch.Tensor,
                           outputs: torch.Tensor,
                           callback_state,
                           *args, **kwargs) -> None:
        metrics = self.on_batch_end(yb, outputs)

        for name, value in metrics.items():
            callback_state.batch_train_criteria[name].append(value)

    def on_eval_batch_end(self,
                           yb: torch.Tensor,
                           outputs: torch.Tensor,
                           callback_state,
                           *args, **kwargs) -> None:
        metrics = self.on_batch_end(yb, outputs)

        for name, value in metrics.items():
            callback_state.batch_eval_criteria[name].append(value)


    def on_train_epoch_end(self,
                     callback_state: CallbackState = None,
                     *args, **kwargs) -> None:
        callback_state.accumulate_train_batch_to_epoch()
        callback_state.clear_train_batch_criteria()

    def on_eval_epoch_end(self,
                     callback_state,
                     *args, **kwargs) -> None:
        callback_state.accumulate_train_batch_to_epoch()
        callback_state.clear_train_batch_criteria()


class PrintCriteriaCallback(BlissCallback):
    def on_train_batch_end(self,
                     yb: torch.Tensor,
                     outputs: torch.Tensor,
                     callback_state,
                     *args, **kwargs) -> None:
        pass

    def on_eval_batch_end(self,
                     yb: torch.Tensor,
                     outputs: torch.Tensor,
                     callback_state,
                     *args, **kwargs) -> None:
        pass

    def on_train_epoch_end(self,
                     callback_state,
                     *args, **kwargs) -> None:
        print(f'loss_function: {callback_state.train_loss_function_values[-1]:.3f}')
        for criteria_name, value in callback_state.get_last_train_criteria_values().items():
            print(f'{criteria_name}: {value:.3f}')
        print()

    def on_eval_epoch_end(self,
                     callback_state,
                     *args, **kwargs) -> None:
        print(f'loss_function: {callback_state.eval_loss_function_values[-1]:.3f}')
        for criteria_name, value in callback_state.get_last_eval_criteria_values().items():
            print(f'{criteria_name}: {value:.3f}')
        print()

