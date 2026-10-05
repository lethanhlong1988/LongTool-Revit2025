using Autodesk.Revit.DB;

namespace LongTool.Services.Dimensions.Layout
{
    /// <summary>
    /// Quy cách đặt dim (khoảng cách, độ dài đường dim...).
    /// Đơn vị public là mm; các property `*Internal` trả về internal units.
    /// </summary>
    public class DimensionLayoutOptions
    {
        // Khoảng cách từ mặt đối tượng đến chân dim (mm)
        public double GapFromObjectMm { get; set; } = 2.0;

        // Hệ số nhân leg length cho lớp dim thứ 2 (mm)
        // (thực tế là offset giữa lớp 1 và lớp 2)
        public double Layer2OffsetFactor { get; set; } = 1.0;

        // Nửa độ dài đường dim tối thiểu (mm)
        public double MinHalfLineMm { get; set; } = 150.0;

        // Hệ số nhân với kích thước lớn nhất của mặt cắt để tính halfLine
        public double HalfLineFactor { get; set; } = 1.5;

        // -------------------- Internal units --------------------

        public double GapFromObjectInternal =>
            UnitUtils.ConvertToInternalUnits(GapFromObjectMm, UnitTypeId.Millimeters);

        public double MinHalfLineInternal =>
            UnitUtils.ConvertToInternalUnits(MinHalfLineMm, UnitTypeId.Millimeters);
    }
}