using CsharpBackend.Models;
using Emgu.CV;
using CsharpBackend.NeuralNetwork;
using Emgu.CV.CvEnum;
using CsharpBackend.Utils;

namespace TestBackend
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
            int size = (int)(1024 * 3.5);
            var neuralNetwork = new NeuralNetwork(
                @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\resources\unetppforcsharp.onnx",
                size
            );
            var coreSampleImage = new CoreSampleImage()
            {
                PathToImage = @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\data\images\Kondur3_122842_1.jpg"
            };

            var image = coreSampleImage.GetImageMat();

            //image = NeuralNetwork.ResizeImage(image, 400, 200, 3);

            var inputImage = await NeuralNetwork.ImagePreprocessing(image);
            var mask = await NeuralNetwork.ProcessImageWithNN(inputImage);
            var maskResized = DataConverter.ResizeImage(mask, image.Cols, image.Rows, 3);

            var pathToImage = @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\data\test\image.jpg";
            var pathToMask = @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\data\test\mask.png";

            image.Save(pathToImage);
            maskResized.Save(pathToMask);

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