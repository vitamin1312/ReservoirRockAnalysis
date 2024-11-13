using Emgu.CV;
using System.CodeDom.Compiler;

namespace CsharpBackend.Utils
{
    public class TmpFiles
    {
        static public string SaveMat(Mat image)
        {
            var tempFiles = new TempFileCollection();
            string file = tempFiles.AddExtension("jpg");
            image.Save(file);
            return file;
        }
    }
}
