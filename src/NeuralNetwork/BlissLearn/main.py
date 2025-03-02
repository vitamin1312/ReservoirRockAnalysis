import torch
import torch.nn as nn
import torch.optim as optim
import matplotlib.pyplot as plt
from torch.utils.data import DataLoader, TensorDataset, random_split
from BlissLearner import BlissLearner
from CustomBlissCallbacks import SegmentationMetricsCallback, PrintCriteriaCallback

torch.manual_seed(42)

# Генерируем более сложные данные
X = torch.randn(500, 2)
y = torch.tensor((X[:, 0]**2 + X[:, 1]**2 > 1).long() + (X[:, 0] + X[:, 1] > 1).long())

dataset = TensorDataset(X, y)
train_size = int(0.8 * len(dataset))
test_size = len(dataset) - train_size
train_dataset, test_dataset = random_split(dataset, [train_size, test_size])

train_loader = DataLoader(train_dataset, batch_size=32, shuffle=True)
test_loader = DataLoader(test_dataset, batch_size=32, shuffle=False)

# Усложняем модель
class MultiClassClassifier(nn.Module):
    def __init__(self):
        super().__init__()
        self.model = nn.Sequential(
            nn.Linear(2, 16),
            nn.ReLU(),
            nn.Linear(16, 16),
            nn.ReLU(),
            nn.Linear(16, 3)  # Три класса
        )

    def forward(self, x):
        return self.model(x)


model = MultiClassClassifier()
criterion = nn.CrossEntropyLoss()
metric = nn.CrossEntropyLoss()  # Будем логировать ту же метрику, что и loss
mse = nn.MSELoss()

def my_metric(yb, outputs):
    return metric(outputs, yb).item()

def my_mse(yb, outputs):
    return mse(yb.float(), outputs.float()).item()


callbacks = [
    SegmentationMetricsCallback(num_classes=3,
                                common_metrics={'CrossEntropy': my_metric},
                                class_metrics={'MSE': my_mse}),
    PrintCriteriaCallback()
]

learner = BlissLearner(model,
                       criterion,
                       optim.Adam,  # Adam лучше подходит для нелинейных задач
                       {'lr': 0.01},
                       train_loader,
                       test_loader,
                       callbacks
                       )

learner.fit(10)  # Увеличим количество эпох

# Визуализация границы принятия решений
plt.figure(figsize=(8, 6))
xx, yy = torch.meshgrid(torch.linspace(-3, 3, 100), torch.linspace(-3, 3, 100))
grid = torch.cat((xx.reshape(-1, 1), yy.reshape(-1, 1)), dim=1)
with torch.no_grad():
    preds = model(grid).argmax(dim=1).reshape(xx.shape)

plt.contourf(xx, yy, preds.numpy(), alpha=0.3)
plt.scatter(X[:, 0].numpy(), X[:, 1].numpy(), c=y.numpy(), edgecolors='k', cmap='viridis')
plt.title("Граница принятия решений модели")
plt.show()