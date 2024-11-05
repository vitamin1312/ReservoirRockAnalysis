using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;


namespace CsharpBackend.Models
{
    public class CoreSampleImage
    {
        public int Id { get; set; }

        public string PathToImage { get; set; }

        public string? PathToMask { get; set; }

        public int ImageInfoId { get; set; }

        public ImageInfo ImageInfo { get; set; }


        // Methods

        public CoreSampleImage ()
        {
            PathToImage = Path.Combine(@"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\wwwroot\ImageFiles",
                $"{Guid.NewGuid()}.jpg");
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

        public Mat? GetImageMat ()
        {

            if (!File.Exists(PathToImage))
                return null;

            return new Mat(PathToImage, ImreadModes.Color);
        }

        public Mat? GetMaskMat ()
        {
            if (!File.Exists(PathToMask))
                return null;

            return new Mat(PathToMask, ImreadModes.Grayscale);
        }

        public Mat? GetMaskImageMat()
        {
            Mat Mask = GetMaskMat();

            if (Mask is null || Mask.IsEmpty)
            {
                return null;
            }

            return Mask2ImageMask(ref Mask);
        }

        public Mat? GetImageWithMaskMat()
        {
            Mat Image = GetImageMat();
            Mat Mask = GetMaskMat();
            Mat MaskImage = GetMaskImageMat();

            if (Image is null || Image.IsEmpty
                || MaskImage is null || MaskImage.IsEmpty
                || Mask is null || Mask.IsEmpty)
            {
                return null;
            }

            var ImageWithMask = new Image<Bgr, byte>(Mask.Size);
            Image<Bgr, byte> ConvertedImage = Image.ToImage<Bgr, byte>();
            Image<Bgr, byte> ConvertedMaskImage = MaskImage.ToImage<Bgr, byte>();

            using (Image<Gray, byte> ConvertedMask = Mask.ToImage<Gray, byte>())
            {
                for (int j = 0; j < Mask.Cols; ++j)
                {
                    for (int i = 0; i < Mask.Rows; ++i)
                    {
                        if (ConvertedMask.Data[i, j, 0] == 0)
                            SetColorFrom(ref ImageWithMask, ref ConvertedImage, i, j);
                        else
                            SetColorFrom(ref ImageWithMask, ref ConvertedMaskImage, i, j);

                    }
                }
            }

            return ImageWithMask.Mat;

        }

        private void SetColor(
            ref Image<Bgr, byte> Image,
            int i, int j,
            byte b, byte g, byte r
            )
        {
            Image.Data[i, j, 0] = b;
            Image.Data[i, j, 1] = g;
            Image.Data[i, j, 2] = r;
        }

        private void SetColorFrom(
            ref Image<Bgr, byte> ImageToSet,
            ref Image<Bgr, byte> ImageToGet,
            int i, int j)
        {
            ImageToSet.Data[i, j, 0] = ImageToGet.Data[i, j, 0];
            ImageToSet.Data[i, j, 1] = ImageToGet.Data[i, j, 1];
            ImageToSet.Data[i, j, 2] = ImageToGet.Data[i, j, 2];
        }

        private Mat? Mask2ImageMask (ref Mat Mask)
        {


            var ImageMask = new Image<Bgr, byte>(Mask.Size);

            using (var ConvertedMask = Mask.ToImage<Gray, byte>())
            {
                for (int j = 0; j < Mask.Cols; ++j)
                {
                    for (int i = 0; i < Mask.Rows; ++i)
                    {
                        if (ConvertedMask.Data[i, j, 0] == 0)
                            SetColor(ref ImageMask, i, j, 0, 0, 0);
                        else if (ConvertedMask.Data[i, j, 0] == 1)
                            SetColor(ref ImageMask, i, j, 0, 255, 0);
                        else if (ConvertedMask.Data[i, j, 0] == 2)
                            SetColor(ref ImageMask, i, j, 0, 0, 255);
                        else if (ConvertedMask.Data[i, j, 0] == 3)
                            SetColor(ref ImageMask, i, j, 0, 255, 255);
                    }
                }
            }

            return ImageMask.Mat;
        }
    }
}
