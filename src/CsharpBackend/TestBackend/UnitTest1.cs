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
            int size = 256;
            var neuralNetwork = new NeuralNetwork(
                @"C:\Users\vikto\Downloads\unetppforcsharp.onnx",
                size
            );
            var coreSampleImage = new CoreSampleImage()
            {
                PathToImage = @"C:\Users\vikto\Documents\IT\AI\pore_segmentation\images\Kondur3_124583_1.jpg"
            };

            var image = coreSampleImage.GetImageMat();

            //image = NeuralNetwork.ResizeImage(image, 400, 200, 3);

            var inputImage = NeuralNetwork.ImagePreprocessing(image);
            var mask = NeuralNetwork.ProcessImageWithNN(inputImage);
            var maskResized = NeuralNetwork.ResizeImage(mask, image.Cols, image.Rows, 3);

            var pathToImage = @"C:\Users\vikto\Documents\IT\AI\pore_segmentation\test\image.jpg";
            var pathToMask = @"C:\Users\vikto\Documents\IT\AI\pore_segmentation\test\mask.png";

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