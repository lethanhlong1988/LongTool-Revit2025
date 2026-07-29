using System;

namespace LongTool.Tables.Geometry;

/// <summary>
/// Đại diện cho vùng hình học của một Cell.
/// Chỉ chứa dữ liệu hình học, không chứa logic render.
/// </summary>
public sealed class CellRect
{
    /// <summary>
    /// Chỉ số của Cell trong quá trình Build Geometry.
    /// </summary>
    public int CellIndex { get; }

    /// <summary>
    /// Hàng bắt đầu của Cell.
    /// </summary>
    public int RowIndex { get; }

    /// <summary>
    /// Cột bắt đầu của Cell.
    /// </summary>
    public int ColumnIndex { get; }

    /// <summary>
    /// Số hàng mà Cell chiếm.
    /// </summary>
    public int RowSpan { get; }

    /// <summary>
    /// Số cột mà Cell chiếm.
    /// </summary>
    public int ColSpan { get; }

    /// <summary>
    /// Tọa độ trái.
    /// </summary>
    public double Left { get; }

    /// <summary>
    /// Tọa độ trên.
    /// </summary>
    public double Top { get; }

    /// <summary>
    /// Chiều rộng.
    /// </summary>
    public double Width { get; }

    /// <summary>
    /// Chiều cao.
    /// </summary>
    public double Height { get; }

    /// <summary>
    /// Tọa độ phải.
    /// </summary>
    public double Right => Left + Width;

    /// <summary>
    /// Tọa độ dưới.
    /// </summary>
    public double Bottom => Top + Height;

    public CellRect(
        int cellIndex,
        int rowIndex,
        int columnIndex,
        int rowSpan,
        int colSpan,
        double left,
        double top,
        double width,
        double height)
    {
        CellIndex = cellIndex;
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        RowSpan = rowSpan;
        ColSpan = colSpan;

        Left = left;
        Top = top;
        Width = width;
        Height = height;
    }

    public override string ToString()
    {
        return
            $"Cell {CellIndex} " +
            $"R{RowIndex} C{ColumnIndex} " +
            $"({Left}, {Top}) " +
            $"{Width}×{Height}";
    }
}