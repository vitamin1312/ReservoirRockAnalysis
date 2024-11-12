using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Emgu.CV.CvEnum;
using CsharpBackend.Utils;



namespace CsharpBackend.NeuralNetwork
{
    public class NeuralNetwork
    {
        private string PathToModel;
        static private InferenceSession _session;
        static private int ImageSize = 256;
        static private int NumChannels = 3;
        static private int NumClasses = 3;
        static public bool is_ready = false;

        public NeuralNetwork(string pathToModel, int imageSize)
        {
            PathToModel = pathToModel;

            if (!File.Exists(PathToModel))
            {
                return;
            }

            var so = Microsoft.ML.OnnxRuntime.SessionOptions.MakeSessionOptionWithCudaProvider(0);
            so.LogSeverityLevel = OrtLoggingLevel.ORT_LOGGING_LEVEL_VERBOSE;  // verbose to check node placements
            _session = new InferenceSession(PathToModel, so);

            ImageSize = imageSize;

            is_ready = true;
        }

        public async static Task<Mat> ProcessImageWithNN(Mat CoreSampleImage)
        {
            var input = new DenseTensor<float>(new[] { 1, 3, ImageSize, ImageSize });

            DataConverter.MatToTensor(CoreSampleImage, ImageSize, ImageSize, ref input);

            var onnxInput = NamedOnnxValue.CreateFromTensor("input", input);
            
            GC.Collect();
            var onnxInputArray = new[] { onnxInput };
            var result = await Task.Run(() =>
            {
                using var results = _session.Run(onnxInputArray);
                if (results.FirstOrDefault()?.Value is not Tensor<float> output)
                    throw new ApplicationException("Unable to process pictures");
                return DataConverter.TensorToMat(output, ImageSize, ImageSize, NumClasses);
            });
            return result;
        }

        public async static Task<Mat> ImagePreprocessing(Mat CoreSampleImage)
        {
            var result = await Task.Run(() =>
            {
                return DataConverter.NormalizeImage(
                   DataConverter.ResizeImage(
                       DataConverter.CvtBgr2Rgb(CoreSampleImage),
                       ImageSize,
                       ImageSize,
                       NumChannels
                       ),
                   ImageSize,
                   ImageSize
                   );
            });
            return result;
        }
    }
}
