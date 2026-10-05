namespace LongTool.Services.BeamDimension
{
    /// <summary>
    /// Tuỳ chọn cho việc dim dầm.
    /// </summary>
    public class BeamDimensionOptions
    {
        // Ngưỡng chấp nhận dầm gần vuông góc view (cos góc)
        public double PerpendicularTolerance { get; set; } = 0.95;

        // Quy cách đặt dim
        public double GapFromObjectMm { get; set; } = 300.0;
        public double MinHalfLineMm { get; set; } = 150.0;
        public double HalfLineFactor { get; set; } = 3;
        public double Layer2OffsetFactor { get; set; } = 200.0;
    }
}