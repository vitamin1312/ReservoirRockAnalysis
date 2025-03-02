from collections import defaultdict
from BlissTypes import tensor_or_scaler


class CallbackState:
    def __init__(self,
                 stop_training: bool = False
                 ):
        self.stop_training = stop_training

        self.train_loss_function_values = list()
        self.eval_loss_function_values = list()
        self.batch_train_criteria = defaultdict(list)
        self.batch_eval_criteria = defaultdict(list)
        self.epoch_train_criteria = defaultdict(list)
        self.epoch_eval_criteria = defaultdict(list)
        self.other_data = dict()

    @staticmethod
    def _get_mean_value(values: tensor_or_scaler) -> tensor_or_scaler:
        return sum(values) / len(values)

    @staticmethod
    def _get_last_value(values: tensor_or_scaler) -> tensor_or_scaler:
        return values[-1]

    def accumulate_train_batch_to_epoch(self) -> None:
        for name, values in self.batch_train_criteria.items():
            self.epoch_train_criteria[name].append(self._get_mean_value(values) if values else None)

    def get_last_train_criteria_values(self):
        last_values = defaultdict(list)
        for name, values in self.epoch_train_criteria.items():
            last_values[name] = self._get_last_value(values) if values else None
        return last_values

    def get_last_eval_criteria_values(self):
        last_values = defaultdict(list)
        for name, values in self.epoch_eval_criteria.items():
            last_values[name] = self._get_last_value(values) if values else None
        return last_values

    def accumulate_eval_batch_to_epoch(self) -> None:
        for name, values in self.batch_eval_criteria.items():
            self.epoch_eval_criteria[name].append(self._get_mean_value(values) if values else None)

    def clear_train_batch_criteria(self):
        self.batch_train_criteria = defaultdict(list)

    def clear_eval_batch_criteria(self):
        self.batch_eval_criteria = defaultdict(list)
