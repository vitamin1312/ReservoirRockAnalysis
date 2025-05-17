using CsharpBackend.Models;
using Emgu.CV;
using CsharpBackend.NeuralNetwork;
using Emgu.CV.CvEnum;
using CsharpBackend.Utils;
using CsharpBackend.Config;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace TestBackend
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // достаём конфиг
            var appConfig = config.GetSection("AppConfig").Get<AppConfig>();
            if (appConfig == null)
                throw new InvalidOperationException("There are no app config");

            // читаем метаданные
            var poreClassesJson = File.ReadAllText(appConfig.PathToPoreClasses);
            var poreClasses = JsonConvert.DeserializeObject<List<PoreClasses>>(poreClassesJson)?.FirstOrDefault();

            var poreColorsJson = File.ReadAllText(appConfig.PathToPoreColors);
            var poreColors = JsonConvert.DeserializeObject<PoreColors>(poreColorsJson);

            if (poreClasses == null || poreColors == null)
                throw new InvalidOperationException("Error while loading metadata (pore colors and pore classes)");

            // инициализируем
            DataConverter.Init(poreClasses, poreColors);
            PorosityConverter.Init(poreClasses, poreColors);
            int size = (int)(1024);
            var neuralNetwork = new NeuralNetwork(
                @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\resources\unetppforcsharp.onnx",
                size
            );
            var coreSampleImage = new CoreSampleImage()
            {
                PathToImage = @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\data\Images\Sihor\images\318261_2.png"
            };

            var image = DataConverter.GetImageMat(coreSampleImage.PathToImage);

            //image = NeuralNetwork.ResizeImage(image, 400, 200, 3);

            var inputImage = await NeuralNetwork.ImagePreprocessing(image);
            var mask = await NeuralNetwork.ProcessImageWithNN(inputImage);
            var maskResized = DataConverter.ResizeImage(mask, image.Cols, image.Rows, 1);

            var pathToImage = @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\wwwroot\ImageFiles\image.png";
            var pathToMask = @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\wwwroot\ImageFiles\mask.png";

            image = DataConverter.ResizeImage(image, 1024, 1024, 3);
            maskResized = DataConverter.ResizeImage(mask, 1024, 1024, 1);

            image.Save(pathToImage);
            maskResized.Save(pathToMask);

            coreSampleImage.PathToImage = pathToImage;
            coreSampleImage.PathToMask = pathToMask;

            var Image = DataConverter.GetImageMat(coreSampleImage.PathToImage);
            var Mask = DataConverter.GetMaskMat(coreSampleImage.PathToMask);
            var MaskImage = DataConverter.GetMaskImageMat(coreSampleImage.PathToMask);
            var ImageWithMask = DataConverter.GetImageWithMaskMat(coreSampleImage.PathToImage, coreSampleImage.PathToMask);

            var poresInfo = PorosityAnalyzer.CalculatePorosityInfo(Mask, 1);

            CvInvoke.Imshow("image", Image);
            CvInvoke.Imshow("mask", Mask * 255 / 5);
            CvInvoke.Imshow("maskImage", MaskImage);
            CvInvoke.Imshow("ImageWithMask", ImageWithMask);
            CvInvoke.WaitKey(0);
        }
    }
}