import torch
import torch.nn as nn
import torch.optim as optim
import matplotlib.pyplot as plt
from torch.utils.data import DataLoader, TensorDataset, random_split

from BlissLearner import BlissLearner

torch.manual_seed(42)
X = torch.linspace(-5, 5, 100).reshape(-1, 1)
y = (2 * X + 3 + torch.randn_like(X) * 2 > 3).float()

dataset = TensorDataset(X, y)
train_size = int(0.8 * len(dataset))
test_size = len(dataset) - train_size
train_dataset, test_dataset = random_split(dataset, [train_size, test_size])

train_loader = DataLoader(train_dataset, batch_size=16, shuffle=True)
test_loader = DataLoader(test_dataset, batch_size=16, shuffle=False)

class BinaryClassifier(nn.Module):
    def __init__(self):
        super().__init__()
        self.linear = nn.Linear(1, 1)
        self.sigmoid = nn.Sigmoid()

    def forward(self, x):
        return self.sigmoid(self.linear(x))

model = BinaryClassifier()
criterion = nn.BCELoss()

learner = BlissLearner(model,
                       criterion,
                       optim.SGD,
                       {'lr': 0.01},
                       train_loader,
                       test_loader
                       )
learner.fit(100)
model = learner.model

plt.scatter(X.numpy(), y.numpy(), label="True Labels", alpha=0.5)
plt.plot(X.numpy(), (model(X).detach().numpy() > 0.5).astype(int), color='red', label="Predicted Boundary")
plt.legend()
plt.show()
