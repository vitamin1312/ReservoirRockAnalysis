import torch

from .BlissCallback import BlissCallback
from ...CallbackState import CallbackState
from .CallbackTypes import nullable_criterion_dict
from ...DTO import BatchResult, ColorizationBatchResult


def _calculate_common_metrics(
        common_metrics: nullable_criterion_dict,
        yb: torch.Tensor,
        outputs: torch.Tensor
) -> dict[str, float]:
    common_metrics_values = dict()
    for name, metric in common_metrics.items():
        common_metrics_values[name] = metric(yb, outputs)
    return common_metrics_values

################### Common metrics Callback ###################
class CommonMetricsCallback(BlissCallback):
    def __init__(
            self,
            common_metrics: nullable_criterion_dict = None,
    ) -> None:

        self.common_metrics = common_metrics if common_metrics else dict()

    def on_batch_end(self,
                     yb: torch.Tensor,
                     outputs: torch.Tensor
                     ) -> dict[str, float]:
        common_metrics = _calculate_common_metrics(self.common_metrics, yb, outputs)

        return common_metrics

    def on_train_batch_end(self,
                           batch_result: BatchResult,
                           callback_state,
                           # 52
                           *args, **kwargs) -> None:
        metrics = self.on_batch_end(batch_result.yb, batch_result.outputs)

        for name, value in metrics.items():
            callback_state.update_criteria(name, value, train=True)

    def on_eval_batch_end(self,
                           batch_result: BatchResult,
                           callback_state: CallbackState,
                           *args, **kwargs) -> None:
        metrics = self.on_batch_end(batch_result.yb, batch_result.outputs)

        for name, value in metrics.items():
            callback_state.update_criteria(name, value, train=False)


    def on_train_epoch_end(self,
                     callback_state: CallbackState = None,
                     *args, **kwargs) -> None:
        callback_state.accumulate_train_batch_to_epoch()
        callback_state.clear_train_batch_criteria()

    def on_eval_epoch_end(self,
                     callback_state: CallbackState,
                     *args, **kwargs) -> None:
        callback_state.accumulate_eval_batch_to_epoch()
        callback_state.clear_eval_batch_criteria()


################### Classification or segmentation Callback ###################
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

    def _calculate_class_metrics(self, yb: torch.Tensor, outputs: torch.Tensor) -> dict[str, float]:

        class_metrics_values = dict()
        preds = outputs.argmax(dim=1)

        for name, metric in self.class_metrics.items():
            mean_value = 0
            for class_num in range(self.num_classes):
                class_outputs = (preds == class_num)
                class_targets = (yb == class_num)
                metric_value = metric(class_outputs, class_targets)
                if isinstance(metric_value, torch.Tensor):
                    metric_value = metric_value.item()
                class_metrics_values[name + f'_class_{class_num}'] = metric_value
                mean_value += metric_value / self.num_classes
            class_metrics_values[name + '_mean'] = mean_value
        return class_metrics_values


    def on_batch_end(self,
                     yb: torch.Tensor,
                     outputs: torch.Tensor
                     ) -> dict[str, float]:
        common_metrics = _calculate_common_metrics(self.common_metrics, yb, outputs)
        class_metrics = self._calculate_class_metrics(yb, outputs)

        return common_metrics | class_metrics

    def on_train_batch_end(self,
                           batch_result: BatchResult,
                           callback_state,
                           # 52
                           *args, **kwargs) -> None:
        metrics = self.on_batch_end(batch_result.yb, batch_result.outputs)

        for name, value in metrics.items():
            callback_state.update_criteria(name, value, train=True)

    def on_eval_batch_end(self,
                           batch_result: BatchResult,
                           callback_state: CallbackState,
                           *args, **kwargs) -> None:
        metrics = self.on_batch_end(batch_result.yb, batch_result.outputs)

        for name, value in metrics.items():
            callback_state.update_criteria(name, value, train=False)


    def on_train_epoch_end(self,
                     callback_state: CallbackState = None,
                     *args, **kwargs) -> None:
        callback_state.accumulate_train_batch_to_epoch()
        callback_state.clear_train_batch_criteria()

    def on_eval_epoch_end(self,
                     callback_state: CallbackState,
                     *args, **kwargs) -> None:
        callback_state.accumulate_eval_batch_to_epoch()
        callback_state.clear_eval_batch_criteria()


################### Colorization Callback ###################
class ColorizationMetricsCallback(BlissCallback):
    def __init__(self,
                 common_generator_metrics: nullable_criterion_dict = None,
                 common_discriminator_metrics: nullable_criterion_dict = None):
        self._common_generator_metrics = common_generator_metrics if common_generator_metrics else dict()
        self._common_discriminator_metrics = common_discriminator_metrics if common_discriminator_metrics else dict()

    def on_batch_end(self,
                     y_true: torch.Tensor,
                     y_fake: torch.Tensor,
                     y_true_labels_preds: torch.Tensor,
                     y_fake_labels_preds: torch.Tensor
                     ) -> dict[str, float]:
        common_generator_metrics = _calculate_common_metrics(
            self._common_generator_metrics,
            y_true,
            y_fake
        )
        y_true_labels = torch.ones_like(y_true_labels_preds)
        y_fake_labels = torch.zeros_like(y_fake_labels_preds)
        y_preds = torch.cat([(y_fake_labels_preds > 0.5).float(), (y_true_labels_preds > 0.5).float()])
        targets = torch.cat([y_fake_labels, y_true_labels])
        common_discriminator_metrics = _calculate_common_metrics(
            self._common_discriminator_metrics,
            y_preds,
            targets
        )

        return common_generator_metrics | common_discriminator_metrics

    def on_train_batch_end(self,
                           batch_result: ColorizationBatchResult,
                           callback_state: CallbackState,
                           # 52
                           *args, **kwargs) -> None:
        metrics = self.on_batch_end(
            y_true=batch_result.y_true,
            y_fake=batch_result.y_fake,
            y_true_labels_preds=batch_result.y_true_labels_preds,
            y_fake_labels_preds=batch_result.y_fake_labels_preds
        )

        for name, value in metrics.items():
            callback_state.update_criteria(name, value, train=True)

    def on_eval_batch_end(self,
                           batch_result: ColorizationBatchResult,
                           callback_state: CallbackState,
                           *args, **kwargs) -> None:
        metrics = self.on_batch_end(
            y_true=batch_result.y_true,
            y_fake=batch_result.y_fake,
            y_true_labels_preds=batch_result.y_true_labels_preds,
            y_fake_labels_preds=batch_result.y_fake_labels_preds
        )

        for name, value in metrics.items():
            callback_state.update_criteria(name, value, train=False)


    def on_train_epoch_end(self,
                     callback_state: CallbackState = None,
                     *args, **kwargs) -> None:
        callback_state.accumulate_train_batch_to_epoch()
        callback_state.clear_train_batch_criteria()

    def on_eval_epoch_end(self,
                     callback_state: CallbackState,
                     *args, **kwargs) -> None:
        callback_state.accumulate_eval_batch_to_epoch()
        callback_state.clear_eval_batch_criteria()


################### Print Callback ###################
class PrintCriteriaCallback(BlissCallback):
    def on_train_batch_end(self,
                     batch_result: BatchResult,
                     callback_state: CallbackState,
                     *args, **kwargs) -> None:
        pass

    def on_eval_batch_end(self,
                     batch_result: BatchResult,
                     callback_state: CallbackState,
                     *args, **kwargs) -> None:
        pass

    def on_train_epoch_end(self,
                     callback_state: CallbackState,
                     *args, **kwargs) -> None:
        for criteria_name, value in callback_state.get_last_train_loss_values().items():
            print(f'{criteria_name}: {value:.3f}')
        for criteria_name, value in callback_state.get_last_train_criteria_values().items():
            print(f'{criteria_name}: {value:.3f}')
        print()

    def on_eval_epoch_end(self,
                     callback_state: CallbackState,
                     *args, **kwargs) -> None:
        for criteria_name, value in callback_state.get_last_eval_loss_values().items():
            print(f'{criteria_name}: {value:.3f}')
        for criteria_name, value in callback_state.get_last_eval_criteria_values().items():
            print(f'{criteria_name}: {value:.3f}')
        print()

