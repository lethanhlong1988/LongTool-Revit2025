using System;
using System.Collections.Generic;

namespace LongTool.Tables.Geometry;

/// <summary>
/// Snapshot hình học của một Table.
/// Chỉ chứa dữ liệu hình học đã được tính toán.
/// Không chứa logic Build hoặc Render.
/// </summary>
public sealed class TableGeometry
{
    private readonly List<CellRect> _cells;

    /// <summary>
    /// Danh sách Cell sau khi tính toán hình học.
    /// </summary>
    public IReadOnlyList<CellRect> Cells =>
        _cells;

    /// <summary>
    /// Số lượng Cell.
    /// </summary>
    public int CellCount =>
        _cells.Count;

    public TableGeometry()
    {
        _cells = new List<CellRect>();
    }

    /// <summary>
    /// Thêm một CellRect.
    /// Chỉ GeometryBuilder được phép sử dụng.
    /// </summary>
    internal void AddCell(
        CellRect cell)
    {
        ArgumentNullException.ThrowIfNull(cell);

        _cells.Add(cell);
    }

    /// <summary>
    /// Xóa toàn bộ CellRect.
    /// </summary>
    internal void Clear()
    {
        _cells.Clear();
    }

    public override string ToString()
    {
        return $"Geometry Cells:{CellCount}";
    }
}