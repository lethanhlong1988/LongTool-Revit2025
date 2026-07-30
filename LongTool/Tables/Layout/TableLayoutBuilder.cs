using System.Collections.Generic;
using LongTool.Tables.Models;

namespace LongTool.Tables.Layouts;

public static class TableLayoutBuilder
{
    public static List<CellLayout> Build(Table table)
    {
        var layouts = new List<CellLayout>();
        var occupied = new bool[table.RowCount, table.ColumnCount];

        double[] columnX = CalculateColumnPositions(table);
        double[] rowY = CalculateRowPositions(table);

        for (int rowIndex = 0; rowIndex < table.RowCount; rowIndex++)
        {
            var row = table.Rows[rowIndex];
            int columnIndex = 0;

            foreach (var cell in row)
            {
                // Tìm vị trí trống
                while (columnIndex < table.ColumnCount && occupied[rowIndex, columnIndex])
                {
                    columnIndex++;
                }

                // Đánh dấu occupied cho merge cell
                for (int r = 0; r < cell.RowSpan; r++)
                {
                    for (int c = 0; c < cell.ColSpan; c++)
                    {
                        occupied[rowIndex + r, columnIndex + c] = true;
                    }
                }

                double width = 0;
                for (int i = 0; i < cell.ColSpan; i++)
                {
                    width += table.Columns[columnIndex + i].Width;
                }

                double height = 0;
                for (int i = 0; i < cell.RowSpan; i++)
                {
                    height += table.Rows[rowIndex + i].Height;
                }

                layouts.Add(new CellLayout
                {
                    Cell = cell,
                    Row = rowIndex,
                    Column = columnIndex,
                    X = columnX[columnIndex],
                    Y = rowY[rowIndex],
                    Width = width,
                    Height = height
                });

                columnIndex += cell.ColSpan;
            }
        }

        return layouts;
    }



    private static double[] CalculateColumnPositions(Table table)
    {
        double[] positions =
            new double[table.ColumnCount];


        double currentX = 0;


        for (int i = 0; i < table.ColumnCount; i++)
        {
            positions[i] = currentX;

            currentX += table.Columns[i].Width;
        }


        return positions;
    }



    private static double[] CalculateRowPositions(Table table)
    {
        double[] positions =
            new double[table.RowCount];


        double currentY = 0;


        for (int i = 0; i < table.RowCount; i++)
        {
            positions[i] = currentY;

            currentY += table.Rows[i].Height;
        }


        return positions;
    }
}