import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import Dataset, DataLoader
import numpy as np
import matplotlib.pyplot as plt
from BlissLearn.BlissLearner import BlissColorizationLearner
from BlissLearn.BlissCallbacks.Callbacks import PrintCriteriaCallback, ColorizationMetricsCallback


device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
print(f"Using device: {device}")

# 1. Генерация синтетических данных
class StripesDataset(Dataset):
    def __init__(self, size=256, num_samples=1000):
        self.size = size
        self.num_samples = num_samples
        self.labels = torch.randint(0, 2, (num_samples,))  # 0 - вертикальные, 1 - горизонтальные

    def __len__(self):
        return self.num_samples

    def __getitem__(self, idx):
        img = np.zeros((self.size, self.size, 3), dtype=np.float32)
        label = self.labels[idx]

        # Создаем grayscale полосы
        if label == 0:  # Вертикальные полосы
            for i in range(0, self.size, 20):
                img[:, i:i + 10] = 0.8  # Светлые полосы
        else:  # Горизонтальные полосы
            for i in range(0, self.size, 20):
                img[i:i + 10, :] = 0.8  # Светлые полосы

        # Цветные версии (для обучения)
        color_img = np.zeros((self.size, self.size, 3), dtype=np.float32)
        if label == 0:  # Красные вертикальные полосы
            for i in range(0, self.size, 20):
                color_img[:, i:i + 10, 0] = 1.0  # Красный канал
        else:  # Зелёные горизонтальные полосы
            for i in range(0, self.size, 20):
                color_img[i:i + 10, :, 1] = 1.0  # Зелёный канал

        # Преобразуем в тензоры и меняем порядок размерностей (C, H, W)
        gray_img = torch.from_numpy(img.mean(axis=2)).unsqueeze(0)  # Grayscale
        color_img = torch.from_numpy(color_img).permute(2, 0, 1)  # RGB

        # Нормализуем
        gray_img = (gray_img - 0.5) / 0.5
        color_img = (color_img - 0.5) / 0.5

        return gray_img.to(device), color_img.to(device)


# 2. Создаем модель для колоризации
class AdvancedColorizer(nn.Module):
    def __init__(self):
        super().__init__()

        self.model = nn.Sequential(
            # Вход: [1, H, W]

            # Downsampling часть
            nn.Conv2d(1, 64, kernel_size=4, stride=2, padding=1),  # [64, H/2, W/2]
            nn.LeakyReLU(0.2, inplace=True),

            nn.Conv2d(64, 128, kernel_size=4, stride=2, padding=1),  # [128, H/4, W/4]
            nn.BatchNorm2d(128),
            nn.LeakyReLU(0.2, inplace=True),

            nn.Conv2d(128, 256, kernel_size=4, stride=2, padding=1),  # [256, H/8, W/8]
            nn.BatchNorm2d(256),
            nn.LeakyReLU(0.2, inplace=True),

            # Residual-блоки
            *[self._make_residual_block(256) for _ in range(6)],

            # Upsampling часть
            nn.ConvTranspose2d(256, 128, kernel_size=4, stride=2, padding=1),  # [128, H/4, W/4]
            nn.BatchNorm2d(128),
            nn.ReLU(inplace=True),

            nn.ConvTranspose2d(128, 64, kernel_size=4, stride=2, padding=1),  # [64, H/2, W/2]
            nn.BatchNorm2d(64),
            nn.ReLU(inplace=True),

            nn.ConvTranspose2d(64, 3, kernel_size=4, stride=2, padding=1),  # [3, H, W]
            nn.Tanh()
        ).to(device)

    @staticmethod
    def _make_residual_block(channels):
        return nn.Sequential(
            nn.Conv2d(channels, channels, kernel_size=3, padding=1),
            nn.BatchNorm2d(channels),
            nn.ReLU(inplace=True),
            nn.Conv2d(channels, channels, kernel_size=3, padding=1),
            nn.BatchNorm2d(channels)
        )

    def forward(self, x):
        return self.model(x)


class SimpleDiscriminator(nn.Module):
    def __init__(self, in_channels=1, color_channels=3, features=64):
        super().__init__()

        input_channels = in_channels + color_channels

        self.model = nn.Sequential(
            nn.Conv2d(input_channels, features, kernel_size=3, stride=2, padding=1),
            nn.BatchNorm2d(features),
            nn.LeakyReLU(0.2, inplace=True),

            nn.Conv2d(features, features * 2, kernel_size=3, stride=2, padding=1),
            nn.BatchNorm2d(features * 2),
            nn.LeakyReLU(0.2, inplace=True),

            nn.Conv2d(features * 2, 1, kernel_size=1),
            nn.Sigmoid()
        ).to(device)

    def forward(self, x, y):
        # Конкатенируем по каналам
        x = torch.cat([x, y], dim=1)
        return self.model(x)


# 4. Создаем датасеты и загрузчики
train_dataset = StripesDataset(size=64, num_samples=1000)
test_dataset = StripesDataset(size=64, num_samples=100)

train_loader = DataLoader(train_dataset, batch_size=32, shuffle=True)
test_loader = DataLoader(test_dataset, batch_size=32, shuffle=False)

def accuracy(y_preds: torch.Tensor, y_true: torch.Tensor):
    return (y_preds == y_true).to(torch.float32).mean()


# 5. Инициализация моделей и learner
generator = AdvancedColorizer()
discriminator = SimpleDiscriminator()
callbacks = [
    PrintCriteriaCallback(),
    ColorizationMetricsCallback(
        common_generator_metrics={'MSE': nn.MSELoss()},
        common_discriminator_metrics={'Accuracy': accuracy}
    )
]

learner = BlissColorizationLearner(
    generator=generator,
    discriminator=discriminator,
    generator_loss_function=nn.L1Loss(),
    discriminator_loss_function=nn.BCELoss(),
    generator_optimizer_class=optim.Adam,
    generator_optimizer_kwargs={'lr': 0.0002},
    discriminator_optimizer_class=optim.Adam,
    discriminator_optimizer_kwargs={'lr': 0.0002},
    train_dataloader=train_loader,
    test_dataloader=test_loader,
    callbacks=callbacks,
    alpha=0.1
)

# 6. Обучение
learner.fit(100)


# 7. Визуализация результатов
def visualize_stripes(learner, num_images=5):
    learner._generator.eval()
    with torch.no_grad():
        # Создаем тестовые изображения
        gray_images = []
        color_images = []

        for i in range(num_images):
            # Чередуем вертикальные и горизонтальные полосы
            dataset = StripesDataset(size=64, num_samples=2)
            gray_vert, color_vert = dataset[0]  # Вертикальные
            gray_horz, color_horz = dataset[1]  # Горизонтальные

            gray_images.extend([gray_vert, gray_horz])
            color_images.extend([color_vert, color_horz])

        gray_batch = torch.stack(gray_images)
        color_batch = torch.stack(color_images)
        generated_color = learner._generator(gray_batch)
        fig, axes = plt.subplots(num_images, 3, figsize=(12, num_images * 3))

        for i in range(num_images):
            gray_img = gray_batch[i][0].detach().cpu().numpy()
            axes[i, 0].imshow(gray_img, cmap='gray', vmin=-1, vmax=1)
            axes[i, 0].set_title("Input Grayscale")
            axes[i, 0].axis('off')

            gen_color = generated_color[i].permute(1, 2, 0).detach().cpu().numpy()
            gen_color = (gen_color + 1) / 2  # Из [-1,1] в [0,1]
            axes[i, 1].imshow(gen_color)
            axes[i, 1].set_title("Generated Color")
            axes[i, 1].axis('off')

            real_color = color_batch[i].permute(1, 2, 0).detach().cpu().numpy()
            real_color = (real_color + 1) / 2  # Из [-1,1] в [0,1]
            axes[i, 2].imshow(real_color)
            axes[i, 2].set_title("Real Color")
            axes[i, 2].axis('off')

        plt.tight_layout()
        plt.show()

visualize_stripes(learner, num_images=4)