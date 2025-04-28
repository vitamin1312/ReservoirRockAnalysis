import torch
# https://www.kaggle.com/code/iezepov/fast-iou-scoring-metric-in-pytorch-and-numpy
SMOOTH = 1e-6

def calculate_iou(outputs: torch.Tensor, labels: torch.Tensor):
    # You can comment out this line if you are passing tensors of equal shape
    # But if you are passing output from UNet or something it will most probably
    # be with the BATCH x 1 x H x W shape
    # outputs = outputs.squeeze(1)  # BATCH x 1 x H x W => BATCH x H x W

    # outputs_sum = outputs.sum()
    # labels_sum = labels.sum()
    
    if outputs.sum() == 0:
        return 0.0  # Если нет предсказаний, IoU равен 0
    
    # Логические операции для нахождения пересечения и объединения
    intersection = torch.logical_and(outputs, labels).float().sum((1, 2))  # Пересечение
    union = torch.logical_or(outputs, labels).float().sum((1, 2))         # Объединение
    
    # Расчет IoU с добавлением "сглаживающего" значения для избегания деления на ноль
    iou = (intersection + SMOOTH) / (union + SMOOTH)
    
    return iou.mean().item()  # Возвращаем средний IoU по батчу

def accuracy(y_preds: torch.Tensor, y_true: torch.Tensor):
    return (y_preds == y_true).float().mean().item()
