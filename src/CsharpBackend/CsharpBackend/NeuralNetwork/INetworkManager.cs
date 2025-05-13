namespace CsharpBackend.NeuralNetwork
{
    public interface INetworkManager
    {
        Task Predict(string pathToImage, string pathToTarget);
    }
}
