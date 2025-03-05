import torch
import numpy as np
from typing import Union, Type
from torch.optim.lr_scheduler import _LRScheduler
from torch import optim


# Python
nullable_int = Union[None, int]

# PyTorch
optimizer_type = Type[optim.Optimizer]
tensor_or_scaler = Union[torch.Tensor, np.array, float]
nullable_scheduler = Union[None, _LRScheduler]
