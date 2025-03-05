from typing import Callable
from .BlissCommonTypes import tensor_or_scaler


# Functions
criterion = Callable[[tensor_or_scaler, tensor_or_scaler], tensor_or_scaler]
