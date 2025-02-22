import torch
import numpy as np
from typing import Callable, Union, Type
from torch.optim.lr_scheduler import _LRScheduler
from torch import optim


tensor_or_scaler = Union[torch.Tensor, np.array, float]
nullable_int = Union[None, int]
criterion = Callable[[tensor_or_scaler, tensor_or_scaler], tensor_or_scaler]
optimizer_type = Type[optim.Optimizer]
nullable_scheduler = Union[None, _LRScheduler]
nullable_list = Union[None, list]
nullable_criterion_list = Union[None, list[criterion]]
step_function = Callable[[torch.Tensor, torch.Tensor], float]