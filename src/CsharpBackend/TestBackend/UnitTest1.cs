using CsharpBackend.Models;
using Emgu.CV;

namespace TestBackend
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var coreSampleManager = new CoreSampleImage()
            {
                PathToImage = @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\data\images\Kondur3_122842_1.jpg",
                PathToMask = @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\data\masks\Kondur3_122842_1.png"
            };

            var result = coreSampleManager.GetImageWithMaskMat();
            if (result is not null)
            {
                CvInvoke.Imshow("result", result);
                CvInvoke.WaitKey(0);
            }
        }
    }
}