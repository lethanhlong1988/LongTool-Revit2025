using System;

namespace LongTool.Tables.Models;

/// <summary>
/// Đại diện cho một cột trong bảng.
/// </summary>
public sealed class TableColumn
{
    /// <summary>
    /// Chiều rộng của cột.
    /// </summary>
    public double Width { get; internal set; }



    /// <summary>
    /// Chỉ số cột trong Table.
    /// Được Table quản lý.
    /// </summary>
    public int Index { get; internal set; }



    public TableColumn(
        double width)
    {
        if (width <= 0)
            throw new ArgumentException(
                "Column width phải lớn hơn 0.",
                nameof(width));


        Width = width;
    }




    internal TableColumn(
        int index,
        double width)
    {
        if (width <= 0)
            throw new ArgumentException(
                "Column width phải lớn hơn 0.",
                nameof(width));


        Index = index;
        Width = width;
    }




    /// <summary>
    /// Kiểm tra dữ liệu cột.
    /// </summary>
    public void Validate()
    {
        if (Width <= 0)
        {
            throw new InvalidOperationException(
                "Column width phải lớn hơn 0.");
        }
    }



    public override string ToString()
    {
        return
            $"Column {Index}, Width:{Width}";
    }
}

