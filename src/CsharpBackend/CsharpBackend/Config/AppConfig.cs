namespace CsharpBackend.Config
{
    public class AppConfig
    {
        public string PathToModel { get; set; }
        public int ImageSize { get; set; }
        public string PathToWWWROOT { get; set; }
        public int _preprocessingCount { get; set; }
        public int _neuralNetworkCount { get; set; }
        public string PathToPoreClasses { get; set; }
        public string PathToPoreColors { get; set; }
        public int OverlapPixels { get; set; }
        public string PathToWebDist { get; set; }
    }
}
