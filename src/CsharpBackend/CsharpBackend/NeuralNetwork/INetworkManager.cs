namespace CsharpBackend.NeuralNetwork
{
    public interface INetworkManager
    {
        void Predict(string pathToImage, string pathToTarget);
    }
}
