using System.Text.Json.Serialization;


namespace CsharpBackend.Models
{
    public class CoreSampleImage
    {
        public int Id { get; set; }

        [JsonIgnore]
        public string? PathToImage { get; set; }

        [JsonIgnore]
        public string? PathToMask { get; set; }

        public int ImageInfoId { get; set; }

        public ImageInfo ImageInfo { get; set; }

        public ICollection<PoreInfo>? PoresInfo { get; } = new List<PoreInfo>();

        // Methods

        public CoreSampleImage() { }


        public static CoreSampleImage WithImage(string pathToRoot, string extension)
        {
            return new CoreSampleImage
            {
                PathToImage = Path.Combine(
                    pathToRoot,
                    "ImageFiles",
                    $"{Guid.NewGuid()}{extension}"
                )
            };
        }

        public static CoreSampleImage WithMask(string pathToRoot, string extension)
        {
            return new CoreSampleImage
            {
                PathToMask = Path.Combine(
                              pathToRoot,
                              "ImageFiles",
                              $"{Guid.NewGuid()}{extension}"
                          )
            };
        }

        public string GenerateMaskPath(string pathToRoot)
        {
            if (PathToMask == null)
                return Path.Combine(pathToRoot,
                    @"ImageFiles",
                    $"{Guid.NewGuid()}.png");
            else
                return PathToMask;
        }

        public bool DeleteImage()
        {
            if (!File.Exists(PathToImage))
                return false;
            File.Delete(PathToImage);
            return true;

        }

        public bool DeleteMask()
        {
            if (!File.Exists(PathToMask))
                return false;
            File.Delete(PathToMask);
            return true;
        }

        public void DeleteItemFiles ()
        {
            DeleteImage();
            DeleteMask();
        }
    }
}
