using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.ML.OnnxRuntime.Tensors;
using CsharpBackend.Config;
using CsharpBackend.Models;
using System.Drawing;
using System.Threading.Tasks;

namespace CsharpBackend.Utils
{

    public class DataConverter : PorosityConverter
    {
        public static Mat CvtBgr2Rgb(Mat CoreSampleImage)
        {
            var RgbImage = new Mat(CoreSampleImage.Cols, CoreSampleImage.Rows, DepthType.Cv8U, 3);
            CvInvoke.CvtColor(CoreSampleImage, RgbImage, ColorConversion.Bgr2Rgb);
            return RgbImage;
        }

        public static Mat ResizeImage(Mat Image, int NumCols, int NumRows, int NumImgChannels)
        {
            var ResizedImage = new Mat(NumCols, NumRows, DepthType.Cv8U, NumImgChannels);
            CvInvoke.Resize(Image, ResizedImage, new System.Drawing.Size(NumCols, NumRows));
            return ResizedImage;
        }

        public static Mat ThumbImage(Mat Image, int NumCols)
        {
            int k = Image.Cols / NumCols;
            int NumRows = Image.Rows / k;
            NumCols = Image.Cols / k;

            var ResizedImage = new Mat(NumRows, NumCols, DepthType.Cv8U, Image.NumberOfChannels);
            CvInvoke.Resize(Image, ResizedImage, new System.Drawing.Size(NumCols, NumRows));
            return ResizedImage;
        }

        public static Mat NormalizeImage(Mat CoreSampleImage, int NRows, int NCols)
        {
            var FloatImage = new Mat(NRows, NCols, DepthType.Cv32F, 3);
            CoreSampleImage.ConvertTo(FloatImage, DepthType.Cv32F);
            FloatImage = FloatImage / 255;

            return FloatImage;
        }


        public static Mat? GetImageMat(string PathToImage)
        {

            if (!File.Exists(PathToImage))
                return null;

            return new Mat(PathToImage, ImreadModes.Color);
        }

        public static Mat? GetMaskMat(string PathToMask)
        {
            if (!File.Exists(PathToMask))
                return null;

            return new Mat(PathToMask, ImreadModes.Grayscale);
        }

        public static Mat? GetMaskImageMat(string PathToMask)
        {
            Mat? Mask = GetMaskMat(PathToMask);

            if (Mask is null || Mask.IsEmpty)
            {
                return null;
            }

            return Mask2ImageMask(ref Mask);
        }

        public static Mat? GetImageWithMaskMat(string PathToImage, string PathToMask)
        {
            Mat? Image = GetImageMat(PathToImage);
            Mat? Mask = GetMaskMat(PathToMask);
            Mat? MaskImage = GetMaskImageMat(PathToMask);

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

        private static void SetColor(
            ref Image<Bgr, byte> Image,
            int i, int j,
            byte b, byte g, byte r
            )
        {
            Image.Data[i, j, 0] = b;
            Image.Data[i, j, 1] = g;
            Image.Data[i, j, 2] = r;
        }

        private static bool IsEqual(
            ref Image<Bgr, byte> Image,
            int i, int j,
            byte[] bgrColor
            )
        {
            return (
                    Image.Data[i, j, 0] == bgrColor[0] &&
                    Image.Data[i, j, 1] == bgrColor[1] &&
                    Image.Data[i, j, 2] == bgrColor[2]
                );
        }


        // todo: convert byte array to bgr object
        private static void SetColor(
            ref Image<Bgr, byte> Image,
            int i, int j,
            byte[] bgrColor
            )
        {
            Image.Data[i, j, 0] = bgrColor[0];
            Image.Data[i, j, 1] = bgrColor[1];
            Image.Data[i, j, 2] = bgrColor[2];
        }

        private static void SetColorFrom(
            ref Image<Bgr, byte> ImageToSet,
            ref Image<Bgr, byte> ImageToGet,
            int i, int j)
        {
            ImageToSet.Data[i, j, 0] = ImageToGet.Data[i, j, 0];
            ImageToSet.Data[i, j, 1] = ImageToGet.Data[i, j, 1];
            ImageToSet.Data[i, j, 2] = ImageToGet.Data[i, j, 2];
        }

        public static Mat? Mask2ImageMask(ref Mat Mask)
        {


            var ImageMask = new Image<Bgr, byte>(Mask.Size);
            byte[] bgrColor;

            using (var ConvertedMask = Mask.ToImage<Gray, byte>())
            {
                for (int j = 0; j < Mask.Cols; ++j)
                {
                    for (int i = 0; i < Mask.Rows; ++i)
                    {
                        bgrColor = IndexColorMap[ConvertedMask.Data[i, j, 0]];
                        SetColor(ref ImageMask, i, j, bgrColor);
                    }
                }
            }

            return ImageMask.Mat;
        }

        public static Mat BgrBinarization(Mat image, byte threshold = 128)
        {
            // Преобразуем в Image<,> для удобного доступа
            var img = image.ToImage<Bgr, byte>();
            for (int i = 0; i < img.Rows; i++)
                for (int j = 0; j < img.Cols; j++)
                    for (int c = 0; c < 3; c++)
                        img.Data[i, j, c] = (img.Data[i, j, c] < threshold) ? (byte)0 : (byte)255;
            return img.Mat;
        }

        public static Mat BgraBinarization(Mat image, byte threshold = 128)
        {
            var img = image.ToImage<Bgra, byte>();

            for (int i = 0; i < img.Rows; i++)
            {
                for (int j = 0; j < img.Cols; j++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        img.Data[i, j, c] = (img.Data[i, j, c] < threshold) ? (byte)0 : (byte)255;
                    }

                    if (img.Data[i, j, 3] == 0)
                    {
                        img.Data[i, j, 0] = 0;
                        img.Data[i, j, 1] = 0;
                        img.Data[i, j, 2] = 0;
                    }
                }
            }

            return img.Mat;
        }

        public static Mat ConvertBgra2Bgr(Mat bgra)
        {
            var bgr = new Mat();
            CvInvoke.CvtColor(bgra, bgr, ColorConversion.Bgra2Bgr);
            return bgr;
        }

        public static Mat ImageMaskToMask(Mat imageMask)
        {
            Mat processed;
            if (imageMask.NumberOfChannels == 4)
            {
                processed = BgraBinarization(imageMask);
                processed = ConvertBgra2Bgr(processed);
            }
            else
            {
                processed = BgrBinarization(imageMask);
            }

            var mask = new Mat(imageMask.Size, DepthType.Cv8U, 1);
            mask.SetTo(new MCvScalar(0));
            var imgMask = mask.ToImage<Gray, byte>();
            var imgProcessed = processed.ToImage<Bgr, byte>();

            foreach (var kvp in IndexColorMap)
            {
                byte[] targetColor = kvp.Value;
                int index = kvp.Key;

                for (int i = 0; i < imgProcessed.Rows; i++)
                {
                    for (int j = 0; j < imgProcessed.Cols; j++)
                    {
                        if (IsEqual(ref imgProcessed, i, j, targetColor))
                            imgMask.Data[i, j, 0] = (byte)index;
                    }
                }
            }
            return imgMask.Mat;
        }


        /// <summary>
        /// Convert Mat to tensor
        /// </summary>
        /// <param name="Mat"></param>
        public static void MatToTensor(Mat CoreSampleImage, int resizeWidth, int resizeHeight, ref DenseTensor<float> input)
        {
            var ConvertedImage = CoreSampleImage.ToImage<Bgr, float>();

            for (int i = 0; i < CoreSampleImage.Rows; ++i)
            {
                for (int j = 0; j < CoreSampleImage.Cols; ++j)
                {
                    input[0, 0, i, j] = ConvertedImage.Data[i, j, 0];
                    input[0, 1, i, j] = ConvertedImage.Data[i, j, 1];
                    input[0, 2, i, j] = ConvertedImage.Data[i, j, 2];
                }
            }
        }

        /// <summary>
        /// Convert tensor to the corresponding bitmap
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Mat TensorToMat(Tensor<float> output, int width, int height, int NumClasses)
        {

            float MaxValue = 0;
            int MaxValueIdx = 0;

            Image<Gray, byte> Mask = new Image<Gray, byte>(width, height);

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    MaxValueIdx = 0;
                    MaxValue = output[0, 0, i, j];

                    for (int c = 1; c < NumClasses; c++)
                    {
                        if (output[0, c, i, j] > MaxValue)
                        {
                            MaxValue = output[0, c, i, j];
                            MaxValueIdx = c;
                        }
                    }

                    Mask.Data[i, j, 0] = (byte)MaxValueIdx;
                }
            }
            return Mask.Mat;
        }
    }
}
