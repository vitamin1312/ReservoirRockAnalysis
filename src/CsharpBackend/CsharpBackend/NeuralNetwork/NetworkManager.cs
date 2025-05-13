using CsharpBackend.Config;
using CsharpBackend.Utils;
using Emgu.CV;
using Emgu.CV.Ocl;
using Emgu.CV.Structure;
using Microsoft.Extensions.Options;


namespace CsharpBackend.NeuralNetwork
{
    public class NetworkManager : INetworkManager
    {
        private NeuralNetwork model;
        private SemaphoreSlim _preprocessingSemaphore;
        private SemaphoreSlim _neuralNetworkSemaphore;
        private readonly int _preprocessingCount = 1;
        private readonly int _neuralNetworkCount = 1;


        public NetworkManager(IOptions<AppConfig> appConfig)
        {
            var config = appConfig.Value;
            model = new NeuralNetwork(config.PathToModel, config.ImageSize);
            _preprocessingCount = config._preprocessingCount;
            _neuralNetworkCount = config._neuralNetworkCount;
            InitSemaphores();
        }
        
        void InitSemaphores()
        {
            _preprocessingSemaphore = new SemaphoreSlim(_preprocessingCount, _preprocessingCount);
            _neuralNetworkSemaphore = new SemaphoreSlim(_neuralNetworkCount, _neuralNetworkCount);
        }

        public async Task Predict(string pathToImage, string pathToTarget)
        {
            var image = new Mat(pathToImage);
            Mat inputImage, mask;

            await _preprocessingSemaphore.WaitAsync();
            try { inputImage = await NeuralNetwork.ImagePreprocessing(image); }
            finally { _preprocessingSemaphore.Release(); }
            if (inputImage.IsEmpty)
                throw new InvalidOperationException("Image preprocessing failed: empty result.");

            await _neuralNetworkSemaphore.WaitAsync();
            try { mask = await NeuralNetwork.ProcessImageWithNN(inputImage); }
            finally { _neuralNetworkSemaphore.Release(); }
            if (mask.IsEmpty)
                throw new InvalidOperationException("Prediction failed: empty result.");

            var maskResized = DataConverter.ResizeImage(mask, image.Cols, image.Rows, 3);
            maskResized.Save(pathToTarget);
        }
    }
}
