using Emgu.CV;
using Emgu.CV.CvEnum;
using System.IO;
using System.Threading.Tasks;


namespace CsharpBackend.Models
{
    public class CoreSampleImage
    {
        public int Id { get; set; }

        public string PathToImage { get; set; }

        public string? PathToMask { get; set; }

        virtual public ImageInfo? Info { get; set; }

        private BitmapFileReaderMat ImageReader = new ();


        // Methods

        public Mat? GetImageMat ()
        {

            if (!File.Exists(PathToImage))
                return null;

            return new Mat(PathToImage, ImreadModes.Color);
        }

        public Mat GetMaskMat ()
        {
            return new Mat(PathToMask, ImreadModes.Grayscale);
        }

        /*public Mat GetMaskImage ()
        {
            Mat Mask = GetImageMat();
            Mat ImageMask = new Mat(Mask.Size, DepthType.Default, 3);

            byte[] MaskArray = new byte[Mask.Rows * Mask.Cols];
            Mask.CopyTo(MaskArray);
            byte PixelValue;

            for (int i = 0; i < Mask.Rows; ++i)
            {
                for (int j = 0; j < Mask.Cols; ++j)
                {
                    PixelValue = MaskArray[i * Mask.Rows + j];

                    switch (PixelValue)
                    {
                        case 0:
                            color 
                    }
                }
            }

            

            return ImageMask;
        }*/
    }
}
