using LongTool.Tables.Engine;
using System.Collections.Generic;
using LongTool.Tables.Models;

namespace LongTool.Tables.Builders;

public static class TableHelper
{
    /// <summary>
    /// Tạo bảng với cấu trúc đơn giản: Header + Data.
    /// </summary>
    public static Table CreateSimpleTable(string[] headers, List<string[]> rows, double colWidth = 50, double rowHeight = 10)
    {
        var builder = new TableBuilder();

        for (int i = 0; i < headers.Length; i++)
        {
            builder.AddColumn(colWidth);
        }

        builder.AddHeaderRow(rowHeight, headers);

        foreach (var row in rows)
        {
            builder.AddDataRow(rowHeight, row);
        }

        return builder.Build();
    }

    /// <summary>
    /// Tạo bảng thống kê đơn giản.
    /// </summary>
    public static Table CreateStatisticsTable(string title, string[] headers, List<string[]> rows)
    {
        var builder = new TableBuilder();

        int colCount = headers.Length;

        for (int i = 0; i < colCount; i++)
        {
            builder.AddColumn(50);
        }

        builder.AddRow(15)
               .AddMergedCell(title, colCount, 1, null, cell =>
               {
                   cell.Style.HorizontalAlignment = TableHorizontalAlignment.Center;
                   cell.Style.VerticalAlignment = TableVerticalAlignment.Middle;
                   cell.Style.Text.Bold = true;
                   cell.Style.Text.FontSize = 4.0;
                   cell.Style.Borders.Top = true;
                   cell.Style.Borders.Bottom = true;
                   cell.Style.Borders.Left = true;
                   cell.Style.Borders.Right = true;
               });

        builder.AddHeaderRow(12, headers);

        foreach (var row in rows)
        {
            builder.AddDataRow(10, row);
        }

        builder.AddTotalRow(12, $"TỔNG CỘNG: {rows.Count} mục", colCount);

        return builder.Build();
    }
}