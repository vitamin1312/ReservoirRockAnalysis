namespace CsharpBackend.Models
{
    public class CoreSampleImage
    {
        public int Id { get; set; }

        public string PathToImage { get; set; }

        public string? PathToMask { get; set; }

        public int ImageInfoId { get; set; }

        virtual public ImageInfo info { get; set; }
    }
}
