using Autodesk.Revit.DB;

namespace LongTool.Services.Dimensions.Layout
{
    /// <summary>
    /// Kết quả tính toán layout cho 1 mặt cắt:
    /// vị trí + hướng + độ dài dim line cho từng nhóm dim.
    /// </summary>
    public class DimensionLayoutResult
    {
        // Dim chiều cao (bên phải mặt cắt)
        public XYZ HeightDimPosition { get; set; }
        public XYZ HeightDimDirection { get; set; }

        // Dim chiều rộng lớp 1 (gần dầm)
        public XYZ WidthLayer1Position { get; set; }
        public XYZ WidthLayer1Direction { get; set; }

        // Dim chiều rộng lớp 2 (xa dầm)
        public XYZ WidthLayer2Position { get; set; }
        public XYZ WidthLayer2Direction { get; set; }

        // Nửa độ dài đường dim (dùng chung cho tất cả)
        public double HalfLine { get; set; }

        // Khoảng cách giữa 2 lớp width (dùng tính lớp 2)
        public double Layer2Offset { get; set; }
    }
}