using System.Text.Json.Serialization;

namespace CsharpBackend.Config
{
    public class PoreClass
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("color")]
        public string Color { get; set; }
        [JsonPropertyName("index")]
        public int Index { get; set; }
    }
    public class PoreClasses
    {
        [JsonPropertyName("Year")]
        public string Year { get; set; }
        [JsonPropertyName("classes")]
        public List<PoreClass> Classes { get; set; }
    }
}
