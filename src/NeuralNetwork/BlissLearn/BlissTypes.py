import torch
import numpy as np
from typing import Callable, Union, Type
from torch.optim.lr_scheduler import _LRScheduler
from torch import optim

from BlissCallback import BlissCallback, BlissSystemCallback

# Python
nullable_int = Union[None, int]

# PyTorch
optimizer_type = Type[optim.Optimizer]
tensor_or_scaler = Union[torch.Tensor, np.array, float]
nullable_scheduler = Union[None, _LRScheduler]

# Functions
step_function = Callable[[torch.Tensor, torch.Tensor], float]
criterion = Callable[[tensor_or_scaler, tensor_or_scaler], tensor_or_scaler]
nullable_system_callbacks_getter = Callable[[], Union[None, list[BlissSystemCallback]]]

# Collections
nullable_list = Union[None, list]
nullable_criterion_dict = Union[None, dict[str, criterion]]
nullable_callbacks_list = Union[None, list[BlissCallback]]
nullable_system_callbacks_list = Union[None, list[BlissSystemCallback]]