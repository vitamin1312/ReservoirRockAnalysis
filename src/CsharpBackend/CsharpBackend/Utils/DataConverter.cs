﻿using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace CsharpBackend.Utils
{
    public class DataConverter
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

        public static Mat NormalizeImage(Mat CoreSampleImage, int NRows, int NCols)
        {
            var FloatImage = new Mat(NRows, NCols, DepthType.Cv32F, 3);
            CoreSampleImage.ConvertTo(FloatImage, DepthType.Cv32F);
            FloatImage = FloatImage / 127.5 - 1;

            return FloatImage;
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
