using LongTool.Tables.Models;

namespace LongTool.Tables.Builders;

public static class TestTableBuilder
{
    public static Table CreateMergeTestTable()
    {
        Table table = new Table();

        // 3 Columns với kích thước khác nhau
        table.AddColumn(2000);
        table.AddColumn(1000);
        table.AddColumn(4000);


        // 7 Rows
        for (int i = 0; i < 7; i++)
        {
            if (i == 2)
            {
                table.AddRow(4000);
            }
            else if (i == 0)
            {
                table.AddRow(1000);
            }
            else
            {
                table.AddRow(500);
            }
        }


        // ==========================
        // Row 1 (index 0): A1 | B1 | C1
        // ==========================

        TableCell cellA1 = table.Rows[0].AddCell();
        cellA1.Name = "Cell_A1";
        cellA1.Text = "A1";

        TableCell cellB1 = table.Rows[0].AddCell();
        cellB1.Name = "Cell_B1";
        cellB1.Text = "B1";

        TableCell cellC1 = table.Rows[0].AddCell();
        cellC1.Name = "Cell_C1";
        cellC1.Text = "C1";


        // ==========================
        // Row 2 (index 1): A2 | B2 | C2
        // ==========================

        TableCell cellA2 = table.Rows[1].AddCell();
        cellA2.Name = "Cell_A2";
        cellA2.Text = "A2";

        TableCell cellB2 = table.Rows[1].AddCell();
        cellB2.Name = "Cell_B2";
        cellB2.Text = "B2";

        TableCell cellC2 = table.Rows[1].AddCell();
        cellC2.Name = "Cell_C2";
        cellC2.Text = "C2";


        // ==========================
        // Row 3 (index 2): A3 | B3+C3 Merge
        // ==========================

        TableCell cellA3 = table.Rows[2].AddCell();
        cellA3.Name = "Cell_A3";
        cellA3.Text = "A3";


        TableCell cellB3 = table.Rows[2].AddCell();
        cellB3.Name = "Cell_B3_C3_Merged";
        cellB3.Text = "B3+C3";
        cellB3.ColSpan = 2;


        // ==========================
        // Row 4 (index 3): A4 | B4 | C4
        // ==========================

        TableCell cellA4 = table.Rows[3].AddCell();
        cellA4.Name = "Cell_A4";
        cellA4.Text = "A4";

        TableCell cellB4 = table.Rows[3].AddCell();
        cellB4.Name = "Cell_B4";
        cellB4.Text = "B4";

        TableCell cellC4 = table.Rows[3].AddCell();
        cellC4.Name = "Cell_C4";
        cellC4.Text = "C4";


        // ==========================
        // Row 5 (index 4): A5 | B5+C5 Merge
        // ==========================

        TableCell cellA5 = table.Rows[4].AddCell();
        cellA5.Name = "Cell_A5";
        cellA5.Text = "A5";


        TableCell cellB5 = table.Rows[4].AddCell();
        cellB5.Name = "Cell_B5_C5_Merged";
        cellB5.Text = "B5+C5";
        cellB5.ColSpan = 2;


        // ==========================
        // Row 6 (index 5): A6 | B6+C6 Merge
        // ==========================

        TableCell cellA6 = table.Rows[5].AddCell();
        cellA6.Name = "Cell_A6";
        cellA6.Text = "A6";


        TableCell cellB6 = table.Rows[5].AddCell();
        cellB6.Name = "Cell_B6_C6_Merged";
        cellB6.Text = "B6+C6";
        cellB6.ColSpan = 2;


        // ==========================
        // Row 7 (index 6): A7 | B7+C7 Merge
        // ==========================

        TableCell cellA7 = table.Rows[6].AddCell();
        cellA7.Name = "Cell_A7";
        cellA7.Text = "A7";


        TableCell cellB7 = table.Rows[6].AddCell();
        cellB7.Name = "Cell_B7_C7_Merged";
        cellB7.Text = "B7+C7";
        cellB7.ColSpan = 2;


        return table;
    }
}