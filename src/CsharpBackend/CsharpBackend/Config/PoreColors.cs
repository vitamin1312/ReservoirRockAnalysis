namespace CsharpBackend.Config
{
    public class PoreColors : Dictionary<string, int[]>
    {
        public (int, int, int) GetBGR(string name)
        {
            if (!TryGetValue(name, out var rgb) || rgb.Length != 3)
                throw new KeyNotFoundException($"Color '{name}' not found or invalid RGB format.");

            return (rgb[2], rgb[1], rgb[0]); // B, G, R
        }
    }
}
