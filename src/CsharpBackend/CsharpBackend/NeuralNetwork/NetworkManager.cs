using CsharpBackend.Utils;
using Emgu.CV;
using Emgu.CV.Ocl;
using Microsoft.Extensions.Options;


namespace CsharpBackend.NeuralNetwork
{
    public class NetworkManager : INetworkManager
    {
        private NeuralNetwork model;

        public NetworkManager(IOptions<AppConfig> appConfig)
        {
            var config = appConfig.Value;
            model = new NeuralNetwork(config.PathToModel, config.ImageSize);
        }

        public async void Predict(string pathToImage, string pathToTarget)
        {
            var image = new Mat(pathToImage);
            var inputImage = await NeuralNetwork.ImagePreprocessing(image);
            var mask = await NeuralNetwork.ProcessImageWithNN(inputImage);
            var maskResized = DataConverter.ResizeImage(mask, image.Cols, image.Rows, 3);
            maskResized.Save(pathToTarget);
        }
    }
}
