using LongTool.Models;
using LongTool.Tables.Models;
using LongTool.Tables.Models.Styles;

namespace LongTool.Tables.Builders;

public static class DoorBoardBuilder
{
    // ==================================================
    // New method
    // Create table from actual DoorScheduleItem
    // ==================================================

    public static Table CreateTable(
        DoorScheduleItem door)
    {
        Table table = new Table();

        // ==================================================
        // Default Style
        // ==================================================

        table.DefaultStyle.CellStyle.HorizontalAlignment =
            TableHorizontalAlignment.Center;

        table.DefaultStyle.CellStyle.VerticalAlignment =
            TableVerticalAlignment.Middle;

        // ==================================================
        // 3 Columns
        // ==================================================

        table.AddColumn(28);
        table.AddColumn(95);
        table.AddColumn(27);

        // ==================================================
        // 8 Rows
        // ==================================================

        table.AddRow(12);
        table.AddRow(12);
        table.AddRow(12);
        table.AddRow(12);
        table.AddRow(12);
        table.AddRow(14);
        table.AddRow(16);
        table.AddRow(150);

        // ==================================================
        // Row 0
        // 記号・数量
        // ==================================================

        TableCell cellA0 =
            table.Rows[0].AddCell();

        cellA0.Name =
            "Cell_A0";

        cellA0.Text =
            "記 号 ・ 数 量";

        TableCell cellB0 =
            table.Rows[0].AddCell();

        cellB0.Name =
            "Cell_B0";

        cellB0.Text =
            door.Symbol;

        cellB0.Style.HorizontalAlignment =
            TableHorizontalAlignment.Left;

        TableCell cellC0 =
            table.Rows[0].AddCell();

        cellC0.Name =
            "Cell_C0";

        cellC0.Text =
            door.Quantity.ToString();

        // ==================================================
        // Row 1
        // 型式
        // ==================================================

        TableCell cellA1 =
            table.Rows[1].AddCell();

        cellA1.Name =
            "Cell_A1";

        cellA1.Text =
            "型 式";

        TableCell cellB1 =
            table.Rows[1].AddCell();

        cellB1.Name =
            "Cell_B1";

        cellB1.Text =
            door.Type;

        cellB1.Style.HorizontalAlignment =
            TableHorizontalAlignment.Left;

        cellB1.ColSpan =
            2;

        // ==================================================
        // Row 2
        // 場所
        // ==================================================

        TableCell cellA2 =
            table.Rows[2].AddCell();

        cellA2.Name =
            "Cell_A2";

        cellA2.Text =
            "場 所";

        TableCell cellB2 =
            table.Rows[2].AddCell();

        cellB2.Name =
            "Cell_B2";

        cellB2.Text =
            door.Location;

        cellB2.Style.HorizontalAlignment =
            TableHorizontalAlignment.Left;

        cellB2.ColSpan =
            2;

        // ==================================================
        // Row 3
        // ガラス
        // ==================================================

        TableCell cellA3 =
            table.Rows[3].AddCell();

        cellA3.Name =
            "Cell_A3";

        cellA3.Text =
            "ガ ラ ス";

        TableCell cellB3 =
            table.Rows[3].AddCell();

        cellB3.Name =
            "Cell_B3";

        cellB3.Text =
            door.Glass;

        cellB3.Style.HorizontalAlignment =
            TableHorizontalAlignment.Left;

        cellB3.ColSpan =
            2;

        // ==================================================
        // Row 4
        // 仕上
        // ==================================================

        TableCell cellA4 =
            table.Rows[4].AddCell();

        cellA4.Name =
            "Cell_A4";

        cellA4.Text =
            "仕 上";

        TableCell cellB4 =
            table.Rows[4].AddCell();

        cellB4.Name =
            "Cell_B4";

        cellB4.Text =
            door.Finish;

        cellB4.Style.HorizontalAlignment =
            TableHorizontalAlignment.Left;

        cellB4.ColSpan =
            2;

        // ==================================================
        // Row 5
        // 金物
        // ==================================================

        TableCell cellA5 =
            table.Rows[5].AddCell();

        cellA5.Name =
            "Cell_A5";

        cellA5.Text =
            "金 物";

        TableCell cellB5 =
            table.Rows[5].AddCell();

        cellB5.Name =
            "Cell_B5";

        cellB5.Text =
            door.Hardware;

        cellB5.Style.HorizontalAlignment =
            TableHorizontalAlignment.Left;

        cellB5.ColSpan =
            2;

        // ==================================================
        // Row 6
        // 備考 + 扉 / 枠
        // ==================================================

        TableCell cellA6 =
            table.Rows[6].AddCell();

        cellA6.Name =
            "Cell_A6";

        cellA6.Text =
            "備 考";

        TableCell cellB6 =
            table.Rows[6].AddCell();

        cellB6.Name =
            "Cell_B6";

        cellB6.Text =
            door.Remarks;

        cellB6.Style.HorizontalAlignment =
            TableHorizontalAlignment.Left;

        TableCell cellC6 =
            table.Rows[6].AddCell();

        cellC6.Name =
            "Cell_C6";

        cellC6.Text =
            $"扉　{door.DoorThickness}\n" +
            $"枠　{door.FrameThickness}";

        cellC6.Style.HorizontalAlignment =
            TableHorizontalAlignment.Left;

        cellC6.Style.VerticalAlignment =
            TableVerticalAlignment.Middle;

        // ==================================================
        // Row 7
        // 形状・寸法
        // ==================================================

        TableCell cellA7 =
            table.Rows[7].AddCell();

        cellA7.Name =
            "Cell_A7";

        cellA7.Text =
            "形 状 ・ 寸 法";

        cellA7.Style.VerticalAlignment =
            TableVerticalAlignment.Top;

        TableCell cellB7 =
            table.Rows[7].AddCell();

        cellB7.Name =
            "Cell_B7_C7_Merged";

        // ==================================================
        // Keep empty for Door Legend
        // ==================================================

        cellB7.Text =
            string.Empty;

        cellB7.ColSpan =
            2;

        cellB7.Style.HorizontalAlignment =
            TableHorizontalAlignment.Left;

        cellB7.Style.VerticalAlignment =
            TableVerticalAlignment.Top;

        return table;
    }


    // ==================================================
    // Compatibility method
    //
    // Existing DrawTableBoardCommand still calls this.
    // It will be removed/replaced in the next step.
    // ==================================================

    public static Table CreateMergeTestTable()
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

        return CreateTable(door);
    }
}