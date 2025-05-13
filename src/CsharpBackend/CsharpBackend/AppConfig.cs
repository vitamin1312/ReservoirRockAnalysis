namespace CsharpBackend
{
    public class AppConfig
    {
        public string PathToModel { get; set; }
        public int ImageSize { get; set; }
        public string PathToWWWROOT { get; set; }

        public int _preprocessingCount {  get; set; }

        public int _neuralNetworkCount {  get; set; }
    }
}
