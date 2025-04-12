from typing import Union, Callable
import torch

from ..BlissCallbacks.Callbacks import BlissCallback
from ..BlissCallbacks.SystemCallbacks import BlissSystemCallback

nullable_callbacks_list = Union[None, list[BlissCallback]]
nullable_system_callbacks_list = Union[None, list[BlissSystemCallback]]
step_function = Callable[[torch.Tensor, torch.Tensor], None]
