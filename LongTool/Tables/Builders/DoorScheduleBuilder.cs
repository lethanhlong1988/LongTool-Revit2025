using LongTool.Tables.Models;
using LongTool.Tables.Models.Styles;

namespace LongTool.Tables.Builders;

public static class DoorScheduleBuilder
{
    public static Table CreateMergeTestTable()
    {
        Table table = new Table();

        table.DefaultStyle.CellStyle.HorizontalAlignment =
            TableHorizontalAlignment.Center;


        table.DefaultStyle.CellStyle.VerticalAlignment =
            TableVerticalAlignment.Middle;

        // 3 Columns với kích thước khác nhau
        table.AddColumn(40);
        table.AddColumn(20);
        table.AddColumn(80);


        // 7 Rows
        for (int i = 0; i < 7; i++)
        {
            if (i == 2)
            {
                table.AddRow(80);
            }
            else if (i == 0)
            {
                table.AddRow(20);
            }
            else if (i == 5 || i == 6)  
            {
                table.AddRow(30); 
            }
            else
            {
                table.AddRow(10);
            }

        }


        // ==========================
        // Row 1 (index 0): A1 | B1 | C1
        // ==========================

        TableCell cellA1 = table.Rows[0].AddCell();
        cellA1.Name = "Cell_A1";
        cellA1.Text = "記号・名称";

        cellA1.Style.HorizontalAlignment =
            TableHorizontalAlignment.Left;

        TableCell cellB1 = table.Rows[0].AddCell();
        cellB1.Name = "Cell_B1";
        cellB1.Text = "SSD /1";

        cellB1.Style.HorizontalAlignment =
            TableHorizontalAlignment.Center;


        TableCell cellC1 = table.Rows[0].AddCell();
        cellC1.Name = "Cell_C1";
        cellC1.Text = "引分け自動ドア";

        cellC1.Style.HorizontalAlignment =
            TableHorizontalAlignment.Right;


        // ==========================
        // Row 2 (index 1): A2 | B2 | C2
        // ==========================

        TableCell cellA2 = table.Rows[1].AddCell();
        cellA2.Name = "Cell_A2";
        cellA2.Text = "数量・位置";

        TableCell cellB2 = table.Rows[1].AddCell();
        cellB2.Name = "Cell_B2";
        cellB2.Text = "1";

        TableCell cellC2 = table.Rows[1].AddCell();
        cellC2.Name = "Cell_C2";
        cellC2.Text = "風除室";


        // ==========================
        // Row 3 (index 2): A3 | B3+C3 Merge
        // ==========================

        TableCell cellA3 = table.Rows[2].AddCell();
        cellA3.Name = "Cell_A3";
        cellA3.Text = "姿図";


        TableCell cellB3 = table.Rows[2].AddCell();
        cellB3.Name = "Cell_B3_C3_Merged";
        cellB3.Text = "※全国自動ドア協会 安全ガイドブックを遵守 防火設備";
        cellB3.ColSpan = 2;


        // ==========================
        // Row 4 (index 3): A4 | B4 | C4
        // ==========================

        TableCell cellA4 = table.Rows[3].AddCell();
        cellA4.Name = "Cell_A4";
        cellA4.Text = "見込・硝子";

        TableCell cellB4 = table.Rows[3].AddCell();
        cellB4.Name = "Cell_B4";
        cellB4.Text = "100";

        TableCell cellC4 = table.Rows[3].AddCell();
        cellC4.Name = "Cell_C4";
        cellC4.Text = "耐熱強化ガラス 5.0＋5.0 FIX部：飛散防止フィルム貼";


        // ==========================
        // Row 5 (index 4): A5 | B5+C5 Merge
        // ==========================

        TableCell cellA5 = table.Rows[4].AddCell();
        cellA5.Name = "Cell_A5";
        cellA5.Text = "仕上";


        TableCell cellB5 = table.Rows[4].AddCell();
        cellB5.Name = "Cell_B5_C5_Merged";
        cellB5.Text = "スチールダイノックシート貼 (枠：SUS HL)";
        cellB5.ColSpan = 2;


        // ==========================
        // Row 6 (index 5): A6 | B6+C6 Merge
        // ==========================

        TableCell cellA6 = table.Rows[5].AddCell();
        cellA6.Name = "Cell_A6";
        cellA6.Text = "金物";


        TableCell cellB6 = table.Rows[5].AddCell();
        cellB6.Name = "Cell_B6_C6_Merged";
        cellB6.Text = "自動ドアエンジン装置、防震枠、ステンレスレール、水抜き配管 付属金物一式 衝突防止シール";
        cellB6.ColSpan = 2;


        // ==========================
        // Row 7 (index 6): A7 | B7+C7 Merge
        // ==========================

        TableCell cellA7 = table.Rows[6].AddCell();
        cellA7.Name = "Cell_A7";
        cellA7.Text = "備考";


        TableCell cellB7 = table.Rows[6].AddCell();
        cellB7.Name = "Cell_B7_C7_Merged";
        cellB7.Text = "飛散防止フィルム、天井センサー感知方式、安全ビーム付、\n電気錠(停電時直前状態保持・火報連動解錠) 戸先ゴム、\n低振動・低騒音型 電気錠 (マジカルテンキー：内・外別番号) 、防潮板、両面シリンダー\"";
        cellB7.ColSpan = 2;


        return table;
    }
}