using System.Diagnostics;
using LongTool.Tables.Models;
using System;

namespace LongTool.Tables.Renderers;

public static class DoorScheduleRenderer
{
    public static void Render(Table table)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));


        Debug.WriteLine("");
        Debug.WriteLine("========== DOOR SCHEDULE RENDERER ==========");


        int rowIndex = 0;


        foreach (var row in table.Rows)
        {
            Debug.WriteLine(
                $"Row {rowIndex} Height:{row.Height}"
            );


            int cellIndex = 0;


            foreach (var cell in row)
            {
                Debug.WriteLine(
                    $"  Cell[{cellIndex}] " +
                    $"Name:{cell.Name} | " +
                    $"Text:{cell.Text} | " +
                    $"Span({cell.RowSpan},{cell.ColSpan})"
                );


                cellIndex++;
            }


            rowIndex++;
        }


        Debug.WriteLine(
            "========== END RENDERER =========="
        );
    }
}