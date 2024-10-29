using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Drawing;
using Emgu.CV.CvEnum;
using System.Security.Policy;



namespace CsharpBackend.NeuralNetwork
{
    public class NeuralNetwork
    {
        private string PathToModel;
        static private InferenceSession _session;
        static private int ImageSize = 256;
        static private int NumClasses = 3;
        public NeuralNetwork(string pathToModel, int imageSize)
        {
            PathToModel = pathToModel;

            if (!File.Exists(PathToModel))
            {
                throw new ApplicationException("Unable to open model");
            }

            //using var gpuSessionOptions = Microsoft.ML.OnnxRuntime.SessionOptions.MakeSessionOptionWithCudaProvider(0);
            _session = new InferenceSession(
                PathToModel
                //gpuSessionOptions
                );

            ImageSize = imageSize;
        }
        public static Mat ProcessImageWithNN(Mat CoreSampleImage)
        {
            var input = new DenseTensor<float>(new[] { 1, 3, ImageSize, ImageSize });

            MatToTensor(CoreSampleImage, ImageSize, ImageSize, ref input);

            var onnxInput = NamedOnnxValue.CreateFromTensor("input", input);
            
            GC.Collect();
            var onnxInputArray = new[] { onnxInput };
            using var results = _session.Run(onnxInputArray);
            if (results.FirstOrDefault()?.Value is not Tensor<float> output)
                throw new ApplicationException("Unable to process pictures");
            return TensorToMat(output, ImageSize, ImageSize);
        }


        /// <summary>
        /// Convert Mat to tensor
        /// </summary>
        /// <param name="Mat"></param>
        private static void MatToTensor(Mat CoreSampleImage, int resizeWidth, int resizeHeight, ref DenseTensor<float> input)
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
        private static Mat TensorToMat(Tensor<float> output, int width, int height)
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

                    for (int c = 1; c < NumClasses; ++c)
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
