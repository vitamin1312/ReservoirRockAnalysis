namespace CsharpBackend.Config
{
    public class PoreColors : Dictionary<string, int[]>
    {
        public byte[] GetBGR(string name)
        {
            if (!TryGetValue(name, out var rgb) || rgb.Length != 3)
                throw new KeyNotFoundException($"Color '{name}' not found or invalid RGB format.");

            return [(byte)rgb[2], (byte)rgb[1], (byte)rgb[0]];
        }
    }
}
