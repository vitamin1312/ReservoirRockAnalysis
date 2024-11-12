using CsharpBackend.Utils;
using Emgu.CV;


namespace CsharpBackend.NeuralNetwork
{
    public class NetworkManager : INetworkManager
    {
        private NeuralNetwork model;

        public NetworkManager()
        {
            model = new NeuralNetwork(@"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\resources\unetppforcsharp.onnx", 2048);
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
