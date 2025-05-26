using CsharpBackend.Config;
using CsharpBackend.Utils;
using Emgu.CV;
using Emgu.CV.Ocl;
using Emgu.CV.Structure;
using Microsoft.Extensions.Options;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace CsharpBackend.NeuralNetwork
{
    public class NetworkManager : INetworkManager
    {
        private NeuralNetwork model;
        private SemaphoreSlim _preprocessingSemaphore;
        private SemaphoreSlim _neuralNetworkSemaphore;
        private readonly int _preprocessingCount;
        private readonly int _neuralNetworkCount;
        private readonly int _imageSize;
        private readonly int _overlapPixels;

        public NetworkManager(IOptions<AppConfig> appConfig)
        {
            var config = appConfig.Value;
            _imageSize = config.ImageSize;
            _overlapPixels = config.OverlapPixels;
            model = new NeuralNetwork(config.PathToModel, _imageSize);
            _preprocessingCount = config._preprocessingCount;
            _neuralNetworkCount = config._neuralNetworkCount;
            InitSemaphores();
        }

        public async Task Predict(string pathToImage, string pathToTarget) // Потрясающий вайб-кодинг)
        {
            using Mat originalImage = new Mat(pathToImage);
            if (originalImage.IsEmpty)
                throw new InvalidOperationException("Input image is empty.");

            if (originalImage.Width < _imageSize || originalImage.Height < _imageSize)
            {
                using Mat smallImageResizedForNN = DataConverter.ResizeImage(originalImage, _imageSize, _imageSize, originalImage.NumberOfChannels);

                Mat nnInputSmall = null;
                try
                {
                    await _preprocessingSemaphore.WaitAsync();
                    try
                    {
                        nnInputSmall = await model.ImagePreprocessing(smallImageResizedForNN);
                    }
                    finally
                    {
                        _preprocessingSemaphore.Release();
                    }

                    if (nnInputSmall == null || nnInputSmall.IsEmpty)
                        throw new InvalidOperationException("Preprocessing failed for small image.");

                    Mat nnOutputMask = null;
                    try
                    {
                        await _neuralNetworkSemaphore.WaitAsync();
                        try
                        {
                            nnOutputMask = await model.ProcessImageWithNN(nnInputSmall);
                        }
                        finally
                        {
                            _neuralNetworkSemaphore.Release();
                        }

                        if (nnOutputMask == null || nnOutputMask.IsEmpty)
                            throw new InvalidOperationException("Prediction failed for small image.");

                        nnOutputMask.Save(pathToTarget);
                    }
                    finally
                    {
                        nnInputSmall?.Dispose();
                    }

                }
                finally
                {
                    // smallImageResizedForNN is disposed by using
                }

                return;
            }

            int windowSize = _imageSize;
            int overlap = _overlapPixels;
            int validStride = windowSize - 2 * overlap;

            if (validStride <= 0)
                throw new ArgumentException($"Overlap ({overlap}) is too large for window size ({windowSize}). It must be less than half of window size.");

            using Mat finalMask = new Mat(originalImage.Rows, originalImage.Cols, Emgu.CV.CvEnum.DepthType.Cv8U, 1);

            for (int y = 0; y < originalImage.Rows; y += validStride)
            {
                for (int x = 0; x < originalImage.Cols; x += validStride)
                {
                    int currentWinX = Math.Max(0, x - overlap);
                    int currentWinY = Math.Max(0, y - overlap);

                    if (currentWinX + windowSize > originalImage.Cols)
                    {
                        currentWinX = originalImage.Cols - windowSize;
                        if (currentWinX < 0) currentWinX = 0;
                    }
                    if (currentWinY + windowSize > originalImage.Rows)
                    {
                        currentWinY = originalImage.Rows - windowSize;
                        if (currentWinY < 0) currentWinY = 0;
                    }

                    Mat subImage = null;
                    Mat preprocessedSubImage = null;
                    Mat segmentationMaskWindow = null;

                    try
                    {
                        subImage = new Mat(originalImage, new Rectangle(currentWinX, currentWinY, windowSize, windowSize));

                        await _preprocessingSemaphore.WaitAsync();
                        try
                        {
                            preprocessedSubImage = await model.ImagePreprocessing(subImage);
                        }
                        finally
                        {
                            _preprocessingSemaphore.Release();
                        }

                        if (preprocessedSubImage == null || preprocessedSubImage.IsEmpty)
                            throw new InvalidOperationException($"Preprocessing failed for window at ({currentWinX},{currentWinY}).");

                        await _neuralNetworkSemaphore.WaitAsync();
                        try
                        {
                            segmentationMaskWindow = await model.ProcessImageWithNN(preprocessedSubImage);
                        }
                        finally
                        {
                            _neuralNetworkSemaphore.Release();
                        }

                        if (segmentationMaskWindow == null || segmentationMaskWindow.IsEmpty)
                            throw new InvalidOperationException($"Prediction failed for window at ({currentWinX},{currentWinY}).");

                        int srcX = x - currentWinX;
                        int srcY = y - currentWinY;
                        int copyRegionWidth = Math.Min(validStride, originalImage.Cols - x);
                        int copyRegionHeight = Math.Min(validStride, originalImage.Rows - y);

                        if (copyRegionWidth <= 0 || copyRegionHeight <= 0)
                            continue;

                        using Mat srcRoiMat = new Mat(segmentationMaskWindow, new Rectangle(srcX, srcY, copyRegionWidth, copyRegionHeight));
                        using Mat dstRoiMat = new Mat(finalMask, new Rectangle(x, y, copyRegionWidth, copyRegionHeight));
                        srcRoiMat.CopyTo(dstRoiMat);
                    }
                    finally
                    {
                        subImage?.Dispose();
                        preprocessedSubImage?.Dispose();
                        segmentationMaskWindow?.Dispose();
                    }
                }
            }

            finalMask.Save(pathToTarget);
        }

        void InitSemaphores()
        {
            _preprocessingSemaphore = new SemaphoreSlim(_preprocessingCount, _preprocessingCount);
            _neuralNetworkSemaphore = new SemaphoreSlim(_neuralNetworkCount, _neuralNetworkCount);
        }
    }
}
