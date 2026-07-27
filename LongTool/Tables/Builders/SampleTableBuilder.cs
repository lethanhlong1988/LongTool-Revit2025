using LongTool.Tables.Engine;
using LongTool.Tables.Models;
using System.Collections.Generic;

namespace LongTool.Tables.Builders;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public static class SampleTableBuilder
{
    public static Table Create()
    {
        var builder = new TableBuilder();

        // 5 cột
        builder.AddColumns(45, 45, 45, 45, 45);

        // Hàng 0
        builder.AddRow(14)
               .AddCrossCell("", CrossLineDirection.TopRightToBottomLeft, "CrossCell", cell =>
               {
                   cell.Style.Borders.Top = true;
                   cell.Style.Borders.Bottom = true;
                   cell.Style.Borders.Left = true;
                   cell.Style.Borders.Right = true;
               })
               .AddMergedCell("HEADER TRÊN", 4, 1, "HeaderTop", cell =>
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

        // Hàng 1
        builder.AddRow(12)
               .AddMergedCell("HEADER TRÁI", 1, 4, "HeaderLeft", cell =>
               {
                   cell.Style.HorizontalAlignment = TableHorizontalAlignment.Center;
                   cell.Style.VerticalAlignment = TableVerticalAlignment.Middle;
                   cell.Style.Text.Bold = true;
                   cell.Style.Text.FontSize = 3.5;
                   cell.Style.Borders.Top = true;
                   cell.Style.Borders.Bottom = true;
                   cell.Style.Borders.Left = true;
                   cell.Style.Borders.Right = true;
               })
               .AddHeader("Cột 1", name: "Col1_Header")
               .AddMergedCell("Cột 2 + 3", 2, 1, "Col23_Header", cell =>
               {
                   cell.Style.HorizontalAlignment = TableHorizontalAlignment.Center;
                   cell.Style.VerticalAlignment = TableVerticalAlignment.Middle;
                   cell.Style.Text.Bold = true;
                   cell.Style.Text.FontSize = 3.5;
                   cell.Style.Borders.Top = true;
                   cell.Style.Borders.Bottom = true;
                   cell.Style.Borders.Left = true;
                   cell.Style.Borders.Right = true;
               })
               .AddHeader("Cột 4", name: "Col4_Header");

        // Hàng 2
        builder.AddRow(10)
               .AddData("Row2-1", name: "Row2_Col1")
               .AddMergedCell("Row2-2+3", 2, 1, "Row2_Col23", cell =>
               {
                   cell.Style.HorizontalAlignment = TableHorizontalAlignment.Center;
                   cell.Style.VerticalAlignment = TableVerticalAlignment.Middle;
                   cell.Style.Borders.Top = true;
                   cell.Style.Borders.Bottom = true;
                   cell.Style.Borders.Left = true;
                   cell.Style.Borders.Right = true;
               })
               .AddData("Row2-4", name: "Row2_Col4");

        // Hàng 3
        builder.AddRow(10)
               .AddData("Row3-1", name: "Row3_Col1")
               .AddMergedCell("Row3-2+3", 2, 1, "Row3_Col23", cell =>
               {
                   cell.Style.HorizontalAlignment = TableHorizontalAlignment.Center;
                   cell.Style.VerticalAlignment = TableVerticalAlignment.Middle;
                   cell.Style.Borders.Top = true;
                   cell.Style.Borders.Bottom = true;
                   cell.Style.Borders.Left = true;
                   cell.Style.Borders.Right = true;
               })
               .AddData("Row3-4", name: "Row3_Col4");

        // Hàng 4
        builder.AddRow(10)
               .AddData("Row4-1", name: "Row4_Col1")
               .AddMergedCell("Row4-2+3", 2, 1, "Row4_Col23", cell =>
               {
                   cell.Style.HorizontalAlignment = TableHorizontalAlignment.Center;
                   cell.Style.VerticalAlignment = TableVerticalAlignment.Middle;
                   cell.Style.Borders.Top = true;
                   cell.Style.Borders.Bottom = true;
                   cell.Style.Borders.Left = true;
                   cell.Style.Borders.Right = true;
               })
               .AddData("Row4-4", name: "Row4_Col4");

        return builder.Build();
    }

    /// <summary>
    /// Tạo bảng chính và bảng debug ID song song.
    /// </summary>
    public static (Table mainTable, Table debugTable, Dictionary<string, (int row, int col)> idMap) CreateWithDebug()
    {
        // 1. Tạo bảng chính
        var mainTable = Create();

        // 2. In ra danh sách ID để debug
        TableDebugHelper.PrintIdMap(mainTable);

        // 3. Tạo bảng debug
        var debugTable = TableDebugHelper.CreateDebugTable(mainTable);

        // 4. Tạo ID map
        var idMap = TableDebugHelper.BuildIdToPositionMap(mainTable);

        return (mainTable, debugTable, idMap);
    }
}