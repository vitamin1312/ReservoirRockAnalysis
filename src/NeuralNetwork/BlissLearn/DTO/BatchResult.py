import torch
from typing import Union, Dict, List
from abc import ABC, abstractmethod

class _BaseBatchResult(ABC):

    def __init__(self,
                 losses: Dict[str, float]
                 ):
        self._losses = losses

    @property
    @abstractmethod
    def losses(self) -> Dict[str, float]:
        pass

    @staticmethod
    def process_number(
            value: Union[float, torch.Tensor]
    ) -> float:
        match value:
            case float():
                return value
            case torch.Tensor():
                return value.item()
            case _:
                raise TypeError(f"Unsupported type: {type(value)}")

    @classmethod
    def dict_from_losses(
            cls,
            losses: Union[float, torch.Tensor, List, Dict]
    ):
        match losses:
            case float():
                return {
                    'loss': losses
                }
            case torch.Tensor():
                return {
                    'loss': losses.item()
                }
            case list():
                return {
                    f'loss_{i}': cls.process_number(loss) for (i, loss) in enumerate(losses)
                }
            case dict():
                return {
                    name: cls.process_number(loss) for name, loss in losses.items()
                }
            case _:
                raise TypeError(f"Unsupported loss container: {type(losses)}")

class BatchResult(_BaseBatchResult):
    def __init__(
            self,
            losses: dict[str, float],
            yb: torch.Tensor,
            outputs: torch.Tensor
            ) -> None:
        super().__init__(
            losses=losses
        )
        self.yb = yb
        self.outputs = outputs
    
    @classmethod
    def from_losses_dict(
        cls,
        losses: Union[float, torch.Tensor, list, dict],
        yb: torch.Tensor,
        outputs: torch.Tensor
    ):
        return cls(
            losses=cls.dict_from_losses(losses),
            yb=yb,
            outputs=outputs,
    )

    @property
    def losses(self):
        return self._losses


class ColorizationBatchResult(_BaseBatchResult):
    def __init__(
            self,
            losses: dict[str, float],
            y_true: torch.Tensor,
            y_fake: torch.Tensor,
            y_true_labels_preds: torch.Tensor,
            y_fake_labels_preds: torch.Tensor
            ) -> None:
        super().__init__(
            losses=losses
        )
        self.y_true = y_true
        self.y_fake = y_fake
        self.y_true_labels_preds = y_true_labels_preds
        self.y_fake_labels_preds = y_fake_labels_preds # 0 - generated image

    @classmethod
    def from_losses_dict(
        cls,
        losses: Union[float, torch.Tensor, list, dict],
        y_true: torch.Tensor,
        y_fake: torch.Tensor,
        y_true_labels_preds: torch.Tensor,
        y_fake_labels_preds: torch.Tensor
    ):
        return cls(
            losses=cls.dict_from_losses(losses),
            y_true=y_true,
            y_fake=y_fake,
            y_true_labels_preds=y_true_labels_preds,
            y_fake_labels_preds=y_fake_labels_preds
    )

    @property
    def losses(self):
        return self._losses