import sys
from collections import defaultdict
import torch

from BatchCallback import BatchCallback
from BlissTypes import nullable_criterion_dict, tensor_or_scaler

class SegmentationMetricsCallback(BatchCallback):
    def __init__(
            self,
            num_classes: int ,
            common_metrics: nullable_criterion_dict = None,
            class_metrics: nullable_criterion_dict = None
    ) -> None:

        self.num_classes = num_classes
        self.common_metrics = common_metrics if common_metrics else dict()
        self.class_metrics = class_metrics if class_metrics else dict()
        self.all_metrics = defaultdict(list)

    def calculate_common_metrics(self, yb: torch.Tensor, outputs: torch.Tensor) -> None:
        for name, metric in self.common_metrics.items():
            self.all_metrics[name].append(metric(yb, outputs))

    def calculate_class_metrics(self, yb: torch.Tensor, outputs: torch.Tensor) -> None:
        for name, metric in self.class_metrics.items():
            for class_num in range(self.num_classes):
                class_outputs= torch.where(outputs == class_num, 1, 0)
                class_targets = torch.where(yb == class_num, 1, 0)
                self.all_metrics[name + f'class{class_num}'].append(metric(class_outputs, class_targets))

    @staticmethod
    def get_mean_value(values: tensor_or_scaler) -> tensor_or_scaler:
        return sum(values) / len(values)

    @staticmethod
    def get_last_value(values: tensor_or_scaler) -> tensor_or_scaler:
        return values[-1]

    def accumulate_metrics_values(self, accumulate_function) -> dict[str, tensor_or_scaler]:
        last_values = dict()

        for name, values in self.all_metrics.items():
            last_values[name] = accumulate_function(values) if values else None

        return last_values

    def on_batch_end(self, yb: torch.Tensor, outputs: torch.Tensor) -> None:
        self.calculate_common_metrics(yb, outputs)
        self.calculate_class_metrics(yb, outputs)

    def on_epoch_end(self):
        mean_values = self.accumulate_metrics_values(self.get_mean_value)
        self.all_metrics = defaultdict(list)
        print(mean_values)

        return mean_values

