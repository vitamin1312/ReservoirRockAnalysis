from torch import optim


class CallbackState:
    def __init__(self,
                 optimizer: optim.Optimizer,
                 loss: float,
                 metrics: dict[str, float],
                 stop_training: bool = False,
                 ):
        self.optimizer = optimizer
        self.loss = loss
        self.metrics = metrics
        self.stop_training = stop_training