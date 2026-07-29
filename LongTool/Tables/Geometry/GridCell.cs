using LongTool.Tables.Models;
using System;

namespace LongTool.Tables.Geometry;

/// <summary>
/// Đại diện cho một ô trong OccupancyGrid.
/// Chỉ sử dụng nội bộ khi xây dựng Geometry.
/// </summary>
internal sealed class GridCell
{
    /// <summary>
    /// Cell đang chiếm ô lưới này.
    /// </summary>
    public TableCell? Cell
    {
        get;
        set;
    }

    /// <summary>
    /// Đây có phải là ô gốc (góc trên trái) của Cell hay không.
    /// </summary>
    public bool IsOrigin
    {
        get;
        set;
    }

    /// <summary>
    /// Ô lưới đã được gán Cell hay chưa.
    /// </summary>
    public bool IsOccupied =>
        Cell != null;

    /// <summary>
    /// Gán Cell cho ô lưới.
    /// </summary>
    public void SetCell(
        TableCell cell,
        bool isOrigin)
    {
        ArgumentNullException.ThrowIfNull(cell);

        Cell = cell;
        IsOrigin = isOrigin;
    }

    /// <summary>
    /// Xóa dữ liệu của ô lưới.
    /// </summary>
    public void Clear()
    {
        Cell = null;
        IsOrigin = false;
    }

    public override string ToString()
    {
        if (Cell == null)
            return "(Empty)";

        return IsOrigin
            ? $"{Cell.Name} (Origin)"
            : $"{Cell.Name}";
    }
}