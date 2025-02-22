from torch import optim


class CallbackState:
    def __init__(self,
                 optimizer: optim.Optimizer,
                 stop_training: bool = False,
                 ):
        self.optimizer = optimizer
        self.stop_training = stop_training
        self.criteria = dict()