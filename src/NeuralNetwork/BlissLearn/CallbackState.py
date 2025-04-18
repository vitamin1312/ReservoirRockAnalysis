from collections import defaultdict


class CallbackState:
    def __init__(self,
                 stop_training: bool = False
                 ) -> None:
        self.stop_training = stop_training
        # loss data
        self.batch_train_loss = defaultdict(list)
        self.batch_eval_loss = defaultdict(list)
        self.epoch_train_loss = defaultdict(list)
        self.epoch_eval_loss = defaultdict(list)

        # criteria data
        self.batch_train_criteria = defaultdict(list)
        self.batch_eval_criteria = defaultdict(list)
        self.epoch_train_criteria = defaultdict(list)
        self.epoch_eval_criteria = defaultdict(list)
        self.other_data = dict()


    ################### Class utils ###################
    @staticmethod
    def _get_mean_value(values: list[float]) -> float:
        return (sum(values) / len(values)) if values else None
    
    def _get_all_criteria(self):
        return {'train info': self.epoch_train_loss | self.epoch_train_criteria, 'eval info': self.epoch_eval_loss | self.epoch_eval_criteria}

    @staticmethod
    def _get_last_value(values: list[float]) -> float:
        return values[-1] if values else None
    
    
    def _accumulate_batch_to_epoch(self,
                                   batch_values: dict[str, list],
                                   epoch_values: dict[str, list]
                                   ) -> None:
        for name, values in batch_values.items():
            epoch_values[name].append(self._get_mean_value(values))

    def accumulate_train_batch_to_epoch(self) -> None:
        self._accumulate_batch_to_epoch(self.batch_train_criteria, self.epoch_train_criteria)

    def accumulate_eval_batch_to_epoch(self) -> None:
        self._accumulate_batch_to_epoch(self.batch_eval_criteria, self.epoch_eval_criteria)

    ################### Criteria processing ###################
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

    ################### Loss processing ###################
    def update_loss_values(self, name: str, value: float, train: bool = True) -> None:
        if train:
            self.batch_train_loss[name].append(value)
        else:
            self.batch_eval_loss[name].append(value)
    
    def accumulate_train_loss_batch_to_epoch(self) -> None:
        self._accumulate_batch_to_epoch(self.batch_train_loss, self.epoch_train_loss)

    def accumulate_eval_loss_batch_to_epoch(self) -> None:
        self._accumulate_batch_to_epoch(self.batch_eval_loss, self.epoch_eval_loss)

    def clear_train_loss_batch(self) -> None:
        self.batch_train_loss = defaultdict(list)

    def clear_eval_loss_batch(self) -> None:
        self.batch_eval_loss = defaultdict(list)

    def get_last_train_loss_values(self) -> dict[str, float]:
        last_values = defaultdict(list)
        for name, values in self.epoch_train_loss.items():
            last_values[name] = self._get_last_value(values) if values else None
        return last_values

    def get_last_eval_loss_values(self) -> dict[str, float]:
        last_values = defaultdict(list)
        for name, values in self.epoch_eval_loss.items():
            last_values[name] = self._get_last_value(values) if values else None
        return last_values
