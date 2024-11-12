using Emgu.CV;
using System.Drawing;


namespace CsharpBackend.NeuralNetwork
{
    public class NetworkManager : INetworkManager
    {
        private NeuralNetwork model;

        public NetworkManager()
        {
            model = new NeuralNetwork(@"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\resources\unetppforcsharp.onnx", 2048);
        }

        public void Predict(string pathToImage, string pathToTarget)
        {
            var image = new Mat(pathToImage);
            var inputImage = NeuralNetwork.ImagePreprocessing(image);
            var mask = NeuralNetwork.ProcessImageWithNN(inputImage);
            var maskResized = NeuralNetwork.ResizeImage(mask, image.Cols, image.Rows, 3);
            maskResized.Save(pathToTarget);
        }
    }
}
