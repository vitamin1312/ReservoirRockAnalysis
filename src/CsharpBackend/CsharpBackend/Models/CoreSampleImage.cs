namespace CsharpBackend.Models
{
    public class CoreSampleImage
    {
        public int Id { get; set; }

        public string PathToImage { get; set; }

        public string? PathToMask { get; set; }

        virtual public ImageInfo Info { get; set; }
    }
}
