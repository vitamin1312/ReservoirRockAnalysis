using ClosedXML.Excel;
using CsharpBackend.Models;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

namespace CsharpBackend.Utils
{
    public class PorosityAnalyzer : PorosityConverter
    {
        public static List<PoreInfo> CalculatePorosityInfo(Mat mask, double pixelLengthRatio, int coreSampleImageId)
        {
            if (_poreClassses == null)
                throw new InvalidOperationException("PorosityConverter not initialized. Call Init() first.");

            List<PoreInfo> poresInfo = new ();

            foreach (var pore in _poreClassses.Classes)
            {
                if (pore.Index == 0) continue;

                var classMask = binaryMaskByIndex(mask, pore.Index);

                using Mat labels = new ();
                using Mat stats = new ();
                using Mat centroids = new ();

                int nLabels = CvInvoke.ConnectedComponentsWithStats(
                    classMask,
                    labels,
                    stats,
                    centroids,
                    LineType.EightConnected,
                    DepthType.Cv32S,
                    ConnectedComponentsAlgorithmsTypes.Default
                );

                int[] statsData = new int[stats.Rows * stats.Cols];
                stats.CopyTo(statsData);

                MCvPoint2D64f[] centroidData = new MCvPoint2D64f[nLabels];
                centroids.CopyTo(centroidData);

                int x, y, width, height, area;

                for (int label = 1; label < nLabels; label++)
                {

                    PoreInfo info = new();

                    area = statsData[label * stats.Cols + 4];

                    x = statsData[label * stats.Cols + 0];
                    y = statsData[label * stats.Cols + 1];
                    width = statsData[label * stats.Cols + 2];
                    height = statsData[label * stats.Cols + 3];

                    var roi = new Rectangle(x, y, width, height);
                    using var roiMat = new Mat(classMask, roi);

                    using var contours = new VectorOfVectorOfPoint();
                    using Mat hier = new ();

                    CvInvoke.FindContours(roiMat, contours, hier, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                    if (contours.Size == 0) continue;

                    VectorOfPoint contour = contours[0];
                    double perimeter = CvInvoke.ArcLength(contour, true);

                    VectorOfPoint convexHull = new ();
                    CvInvoke.ConvexHull(contour, convexHull);

                    var circle = CvInvoke.MinEnclosingCircle(contour);

                    if (contour.Size >= 5)
                    {
                        var ellipse = CvInvoke.FitEllipse(contour);

                        info.AspectRatio = SanitizeDouble(Math.Max(ellipse.Size.Width, ellipse.Size.Height) /
                                                          Math.Min(ellipse.Size.Width, ellipse.Size.Height));

                        info.Orientation = ellipse.Angle;
                    }

                    info.Area = area * pixelLengthRatio * pixelLengthRatio;
                    info.Perimeter = perimeter * pixelLengthRatio;
                    info.Circularity = SanitizeDouble(4 * Math.PI * area / (perimeter * perimeter));

                    info.FeretDiameter = circle.Radius * 2 * pixelLengthRatio;
                    info.ConvexArea = CvInvoke.ContourArea(convexHull) * pixelLengthRatio * pixelLengthRatio;
                    info.CentroidX = centroidData[label].X;
                    info.CentroidY = centroidData[label].Y;
                    info.PorosityName = pore.Name;
                    info.Index = label;
                    info.pixelLengthRatio = pixelLengthRatio;
                    info.CoreSampleImageId = coreSampleImageId;

                    poresInfo.Add(info);
                }

            }

            return poresInfo;
        }

        public static IXLWorksheet ExportPoreInfoToWorksheet(IXLWorkbook workbook, List<PoreInfo> pores)
        {
            var worksheet = workbook.Worksheets.Add("Pore Data");

            var properties = ReflectionUtils.GetPublicProperties<PoreInfo>().ToList();

            for (int i = 0; i < properties.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = properties[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            for (int row = 0; row < pores.Count; row++)
            {
                for (int col = 0; col < properties.Count; col++)
                {
                    var value = ReflectionUtils.GetPropertyValue(pores[row], properties[col]);
                    worksheet.Cell(row + 2, col + 1).Value = value?.ToString() ?? "";
                }
            }

            worksheet.Columns().AdjustToContents();

            return worksheet;
        }

        private static Mat binaryMaskByIndex(Mat mask, int index)
        {
            Mat result = new Mat();
            CvInvoke.InRange(mask, new ScalarArray(new MCvScalar(index)),
                             new ScalarArray(new MCvScalar(index)), result);
            return result;
        }

        private static double? SanitizeDouble(double? value) =>
            value is null || double.IsNaN(value.Value) || double.IsInfinity(value.Value) ? null : value;
    }
}
