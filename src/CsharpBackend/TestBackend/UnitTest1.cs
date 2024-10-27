using CsharpBackend.Models;

namespace TestBackend
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var coreSampleManager = new CoreSampleImage()
            {
                PathToImage = @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\data\Кондурчинская\Kondur3_124774_1.jpg",
                PathToMask = @"C:\Users\Viktor\Documents\IT\ReservoirRockAnalysis\data\Кондурчинская\Kondur3_124774_1.jpg"
            };

            var result = coreSampleManager.GetImageMat();
        }
    }
}