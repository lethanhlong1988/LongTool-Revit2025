using LongTool.Tables.Engine;

namespace LongTool.Tables.Builders;

public static class TableBuilderExtensions
{
    /// <summary>
    /// Thêm một hàng tiêu đề với nhiều cột.
    /// </summary>
    public static TableBuilder AddHeaderRow(this TableBuilder builder, double height, params string[] headers)
    {
        builder.AddRow(height);
        foreach (string header in headers)
        {
            builder.AddHeader(header);
        }
        return builder;
    }

    /// <summary>
    /// Thêm một hàng dữ liệu với nhiều cột.
    /// </summary>
    public static TableBuilder AddDataRow(this TableBuilder builder, double height, params string[] data)
    {
        builder.AddRow(height);
        foreach (string item in data)
        {
            builder.AddData(item);
        }
        return builder;
    }

    /// <summary>
    /// Thêm một hàng dữ liệu với alignment tùy chỉnh.
    /// </summary>
    public static TableBuilder AddDataRow(this TableBuilder builder, double height, (string text, TableHorizontalAlignment align)[] data)
    {
        builder.AddRow(height);
        foreach (var item in data)
        {
            builder.AddData(item.text, item.align);
        }
        return builder;
    }

    /// <summary>
    /// Thêm hàng tổng hợp (merge toàn bộ).
    /// </summary>
    public static TableBuilder AddTotalRow(this TableBuilder builder, double height, string text, int colSpan)
    {
        builder.AddRow(height);
        builder.AddMergedCell(text, colSpan, 1, null, cell =>
        {
            cell.Style.HorizontalAlignment = TableHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = TableVerticalAlignment.Middle;
            cell.Style.Text.Bold = true;
            cell.Style.Text.FontSize = 3.5;
            cell.Style.Borders.Top = true;
            cell.Style.Borders.Bottom = true;
            cell.Style.Borders.Left = true;
            cell.Style.Borders.Right = true;
        });
        return builder;
    }
}