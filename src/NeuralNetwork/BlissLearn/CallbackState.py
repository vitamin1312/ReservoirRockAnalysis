from collections import defaultdict


class CallbackState:
    def __init__(self,
                 stop_training: bool = False
                 ) -> None:
        self.stop_training = stop_training

        self.batch_train_loss = list()
        self.batch_eval_loss = list()
        self.epoch_train_loss = list()
        self.epoch_eval_loss = list()
        self.batch_train_criteria = defaultdict(list)
        self.batch_eval_criteria = defaultdict(list)
        self.epoch_train_criteria = defaultdict(list)
        self.epoch_eval_criteria = defaultdict(list)
        self.other_data = dict()

    @staticmethod
    def _get_mean_value(values: list[float]) -> float:
        return (sum(values) / len(values)) if values else None

    @staticmethod
    def _get_last_value(values: list[float]) -> float:
        return values[-1] if values else None

    def _accumulate_batch_to_epoch(self,
                                   batch_criteria: dict[str, list],
                                   epoch_criteria: dict[str, list]
                                   ) -> None:
        for name, values in batch_criteria.items():
            epoch_criteria[name].append(self._get_mean_value(values))

    def accumulate_train_batch_to_epoch(self) -> None:
        self._accumulate_batch_to_epoch(self.batch_train_criteria, self.epoch_train_criteria)

    def accumulate_eval_batch_to_epoch(self) -> None:
        self._accumulate_batch_to_epoch(self.batch_eval_criteria, self.epoch_eval_criteria)

    def get_last_train_criteria_values(self) -> dict[str, float]:
        last_values = defaultdict(list)
        for name, values in self.epoch_train_criteria.items():
            last_values[name] = self._get_last_value(values) if values else None
        return last_values

    def get_last_eval_criteria_values(self) -> dict[str, float]:
        last_values = defaultdict(list)
        for name, values in self.epoch_eval_criteria.items():
            last_values[name] = self._get_last_value(values) if values else None
        return last_values

    def clear_train_batch_criteria(self) -> None:
        self.batch_train_criteria = defaultdict(list)

    def clear_eval_batch_criteria(self) -> None:
        self.batch_eval_criteria = defaultdict(list)

    def update_criteria(self, name: str, value: float, train: bool = True) -> None:
        if train:
            self.batch_train_criteria[name].append(value)
        else:
            self.batch_eval_criteria[name].append(value)

    def update_loss_values(self, value: float, train: bool = True) -> None:
        if train:
            self.batch_train_loss.append(value)
        else:
            self.batch_eval_loss.append(value)

    def _accumulate_batch_loss_to_epoch(self,
                                        batch_loss: list[float],
                                        epoch_loss: list[float]
                                        ) -> None:
        epoch_loss.append(self._get_mean_value(batch_loss))

    def accumulate_train_loss_batch_to_epoch(self) -> None:
        self._accumulate_batch_loss_to_epoch(self.batch_train_loss, self.epoch_train_loss)

    def accumulate_eval_loss_batch_to_epoch(self) -> None:
        self._accumulate_batch_loss_to_epoch(self.batch_eval_loss, self.epoch_eval_loss)

    def clear_train_loss_batch(self) -> None:
        self.batch_train_loss = list()

    def clear_eval_loss_batch(self) -> None:
        self.batch_eval_loss = list()
