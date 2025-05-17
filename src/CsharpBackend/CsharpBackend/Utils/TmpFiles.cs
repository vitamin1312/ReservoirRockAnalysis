using ClosedXML.Excel;
using Emgu.CV;
using System.CodeDom.Compiler;

namespace CsharpBackend.Utils
{
    public class TmpFiles
    {
        static public string SaveMat(Mat image)
        {
            var tempFiles = new TempFileCollection();
            string file = tempFiles.AddExtension("png");
            image.Save(file);
            return file;
        }

        static public string SaveExcel(XLWorkbook workbook)
        {
            var tempFiles = new TempFileCollection();
            string file = tempFiles.AddExtension("xlsx");
            workbook.SaveAs(file);
            return file;
        }
    }
}
