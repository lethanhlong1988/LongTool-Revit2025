using LongTool.Models;
using LongTool.Tables.Models;
using LongTool.Tables.Models.Styles;
using System;

namespace LongTool.Tables.Builders;

public static class DoorBoardBuilder
{
    // ==================================================
    // Padding cơ sở (đơn vị gốc, sẽ được nhân với scale)
    // ==================================================

    private const double BasePadding = 1.0;

    // ==================================================
    // Helper: áp padding cho một cell
    // ==================================================

    private static void ApplyPadding(
        TableCell cell,
        double scale)
    {
        double padding = BasePadding * scale;

        cell.Style.PaddingLeft = padding;
        cell.Style.PaddingRight = padding;
        cell.Style.PaddingTop = padding;
        cell.Style.PaddingBottom = padding;
    }

    // ==================================================
    // New method
    // ==================================================

    public static Table CreateTable(
        DoorScheduleItem door,
        double scale = 1.0)
    {
        if (scale <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(scale),
                "Scale must be greater than zero.");

        Table table = new Table();

        // ==================================================
        // Default Style
        // ==================================================

        table.DefaultStyle.CellStyle.HorizontalAlignment =
            TableHorizontalAlignment.Center;

        table.DefaultStyle.CellStyle.VerticalAlignment =
            TableVerticalAlignment.Middle;

        table.DefaultStyle.CellStyle.PaddingLeft = BasePadding * scale;
        table.DefaultStyle.CellStyle.PaddingRight = BasePadding * scale;
        table.DefaultStyle.CellStyle.PaddingTop = BasePadding * scale;
        table.DefaultStyle.CellStyle.PaddingBottom = BasePadding * scale;

        // ==================================================
        // 3 Columns
        // ==================================================

        table.AddColumn(28 * scale);
        table.AddColumn(95 * scale);
        table.AddColumn(27 * scale);

        // ==================================================
        // 8 Rows
        // ==================================================

        table.AddRow(12 * scale);
        table.AddRow(12 * scale);
        table.AddRow(12 * scale);
        table.AddRow(12 * scale);
        table.AddRow(12 * scale);
        table.AddRow(14 * scale);
        table.AddRow(16 * scale);
        table.AddRow(150 * scale);

        // ==================================================
        // Row 0
        // ==================================================

        TableCell cellA0 = table.Rows[0].AddCell();
        cellA0.Name = "Cell_A0";
        cellA0.Text = "記 号 ・ 数 量";
        ApplyPadding(cellA0, scale);

        TableCell cellB0 = table.Rows[0].AddCell();
        cellB0.Name = "Cell_B0";
        cellB0.Text = door.Symbol;
        cellB0.Style.HorizontalAlignment = TableHorizontalAlignment.Left;
        ApplyPadding(cellB0, scale);

        TableCell cellC0 = table.Rows[0].AddCell();
        cellC0.Name = "Cell_C0";
        cellC0.Text = door.Quantity.ToString();
        ApplyPadding(cellC0, scale);

        // ==================================================
        // Row 1
        // ==================================================

        TableCell cellA1 = table.Rows[1].AddCell();
        cellA1.Name = "Cell_A1";
        cellA1.Text = "型 式";
        ApplyPadding(cellA1, scale);

        TableCell cellB1 = table.Rows[1].AddCell();
        cellB1.Name = "Cell_B1";
        cellB1.Text = door.Type;
        cellB1.Style.HorizontalAlignment = TableHorizontalAlignment.Left;
        cellB1.ColSpan = 2;
        ApplyPadding(cellB1, scale);

        // ==================================================
        // Row 2
        // ==================================================

        TableCell cellA2 = table.Rows[2].AddCell();
        cellA2.Name = "Cell_A2";
        cellA2.Text = "場 所";
        ApplyPadding(cellA2, scale);

        TableCell cellB2 = table.Rows[2].AddCell();
        cellB2.Name = "Cell_B2";
        cellB2.Text = door.Location;
        cellB2.Style.HorizontalAlignment = TableHorizontalAlignment.Left;
        cellB2.ColSpan = 2;
        ApplyPadding(cellB2, scale);

        // ==================================================
        // Row 3
        // ==================================================

        TableCell cellA3 = table.Rows[3].AddCell();
        cellA3.Name = "Cell_A3";
        cellA3.Text = "ガ ラ ス";
        ApplyPadding(cellA3, scale);

        TableCell cellB3 = table.Rows[3].AddCell();
        cellB3.Name = "Cell_B3";
        cellB3.Text = door.Glass;
        cellB3.Style.HorizontalAlignment = TableHorizontalAlignment.Left;
        cellB3.ColSpan = 2;
        ApplyPadding(cellB3, scale);

        // ==================================================
        // Row 4
        // ==================================================

        TableCell cellA4 = table.Rows[4].AddCell();
        cellA4.Name = "Cell_A4";
        cellA4.Text = "仕 上";
        ApplyPadding(cellA4, scale);

        TableCell cellB4 = table.Rows[4].AddCell();
        cellB4.Name = "Cell_B4";
        cellB4.Text = door.Finish;
        cellB4.Style.HorizontalAlignment = TableHorizontalAlignment.Left;
        cellB4.ColSpan = 2;
        ApplyPadding(cellB4, scale);

        // ==================================================
        // Row 5
        // ==================================================

        TableCell cellA5 = table.Rows[5].AddCell();
        cellA5.Name = "Cell_A5";
        cellA5.Text = "金 物";
        ApplyPadding(cellA5, scale);

        TableCell cellB5 = table.Rows[5].AddCell();
        cellB5.Name = "Cell_B5";
        cellB5.Text = door.Hardware;
        cellB5.Style.HorizontalAlignment = TableHorizontalAlignment.Left;
        cellB5.ColSpan = 2;
        ApplyPadding(cellB5, scale);

        // ==================================================
        // Row 6
        // ==================================================

        TableCell cellA6 = table.Rows[6].AddCell();
        cellA6.Name = "Cell_A6";
        cellA6.Text = "備 考";
        ApplyPadding(cellA6, scale);

        TableCell cellB6 = table.Rows[6].AddCell();
        cellB6.Name = "Cell_B6";
        cellB6.Text = door.Remarks;
        cellB6.Style.HorizontalAlignment = TableHorizontalAlignment.Left;
        ApplyPadding(cellB6, scale);

        TableCell cellC6 = table.Rows[6].AddCell();
        cellC6.Name = "Cell_C6";
        cellC6.Text =
            $"扉　{door.DoorThickness}\n" +
            $"枠　{door.FrameThickness}";
        cellC6.Style.HorizontalAlignment = TableHorizontalAlignment.Left;
        cellC6.Style.VerticalAlignment = TableVerticalAlignment.Middle;
        ApplyPadding(cellC6, scale);

        // ==================================================
        // Row 7
        // ==================================================

        TableCell cellA7 = table.Rows[7].AddCell();
        cellA7.Name = "Cell_A7";
        cellA7.Text = "形 状 ・ 寸 法";
        cellA7.Style.VerticalAlignment = TableVerticalAlignment.Top;
        ApplyPadding(cellA7, scale);

        TableCell cellB7 = table.Rows[7].AddCell();
        cellB7.Name = "Cell_B7_C7_Merged";
        cellB7.Text = string.Empty;
        cellB7.ColSpan = 2;
        cellB7.Style.HorizontalAlignment = TableHorizontalAlignment.Left;
        cellB7.Style.VerticalAlignment = TableVerticalAlignment.Top;
        ApplyPadding(cellB7, scale);

        return table;
    }

    // ==================================================
    // Compatibility method
    // ==================================================

    public static Table CreateMergeTestTable(double scale = 1.0)
    {
        DoorScheduleItem door =
            new DoorScheduleItem
            {
                Symbol = "SD-1",
                Quantity = 1,

                Type =
                    "スチール製片開きフラッシュドア（特定防火設備/常閉）",

                Location =
                    "風除室",

                Glass =
                    "カスミ耐熱結晶化ガラスt=5",

                Finish =
                    "スチール化粧鋼板t=0.6（水晶化アルミコア）焼付塗装",

                Hardware =
                    "丁番、レバーハンドル、戸当り、SUS見切、ドアクローザー\n" +
                    "シリンダー電気錠（内：サムターン）",

                Remarks =
                    "電気錠、カードキー、制御盤（警備保障会社工事）",

                DoorThickness =
                    "40",

                FrameThickness =
                    "165"
            };

        return CreateTable(door, scale);
    }
}