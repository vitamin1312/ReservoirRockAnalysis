using CsharpBackend.Models;
using Emgu.CV;
using CsharpBackend.NeuralNetwork;
using Emgu.CV.CvEnum;

namespace TestBackend
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            int size = 512;
            var neuralNetwork = new NeuralNetwork(
                @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\resources\unetppforcsharp.onnx",
                size
            );
            var coreSampleImage = new CoreSampleImage()
            {
                PathToImage = @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\data\images\Kondur3_122842_1.jpg",
            };

            var image = coreSampleImage.GetImageMat();
            

            var resizedImage = new Mat(size, size, DepthType.Cv8U, 3);
            CvInvoke.Resize(image, resizedImage, new System.Drawing.Size(size, size));
            var rgbImage = new Mat(size, size, DepthType.Cv8U, 3);
            CvInvoke.CvtColor(resizedImage, rgbImage, ColorConversion.Bgr2Rgb);
            var floatImage = new Mat(size, size, DepthType.Cv32F, 3);
            rgbImage.ConvertTo(floatImage, DepthType.Cv32F);
            floatImage = floatImage / 127.5 - 1;


            var mask = NeuralNetwork.ProcessImageWithNN(floatImage);

            var pathToImage = @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\data\test\image.jpg";
            var pathToMask = @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\data\test\mask.png";

            resizedImage.Save(pathToImage);
            mask.Save(pathToMask);

            coreSampleImage.PathToImage = pathToImage;
            coreSampleImage.PathToMask = pathToMask;

            var Image = coreSampleImage.GetImageMat();
            var Mask = coreSampleImage.GetMaskMat();
            var MaskImage = coreSampleImage.GetMaskImageMat();
            var ImageWithMask = coreSampleImage.GetImageWithMaskMat();
            CvInvoke.Imshow("image", Image);
            CvInvoke.Imshow("mask", Mask * 255 / 5);
            CvInvoke.Imshow("maskImage", MaskImage);
            CvInvoke.Imshow("ImageWithMask", ImageWithMask);
            CvInvoke.WaitKey(0);

        }
    }
}