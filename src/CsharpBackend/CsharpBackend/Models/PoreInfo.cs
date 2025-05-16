namespace CsharpBackend.Models
{
    public class PoreInfo
    {
        public int Id { get; set; }

        public string? PorosityName { get; set; }

        public int? Index { get; set; }

        /// Площадь поры в пикселях (кол-во пикселей внутри контура)
        public long? Area { get; set; }

        /// Длина границы поры (периметр контура)
        public long? Perimeter { get; set; }

        /// Оценка "округлости" поры: 4π * Area / Perimeter²
        /// Принимает значение от 0 до 1, где 1 — идеальный круг
        public double? Circularity { get; set; }

        /// Отношение длин осей эллипса, аппроксимирующего пору (Major / Minor)
        /// Чем ближе к 1 — тем ближе к окружности
        public double? AspectRatio { get; set; }

        /// Диаметр Ферета — максимальное расстояние между двумя точками на контуре поры
        /// Аналог "наибольшего диаметра"
        public double? FeretDiameter { get; set; }

        /// Угол наклона главной оси эллипса (в градусах или радианах)
        public double? Orientation { get; set; }

        /// Центр масс (центроид) поры — рассчитывается по моментам
        public int CentroidX { get; set; }
        public int CentroidY { get; set; }

        /// Площадь выпуклой оболочки поры (Convex Hull)
        /// Используется для оценки выпуклости и плотности формы
        public double? ConvexArea { get; set; }

        public int? CoreSampleImageId { get; set; }

        public CoreSampleImage? coreSampleImage { get; set; }
    }

}
