using System;
using System.Collections.Generic;
using System.Diagnostics;
using LongTool.Tables.Layout;

namespace LongTool.Tables.Diagnostics;

public static class LayoutDebugger
{
    public static void Show(
    IReadOnlyList<TableCellLayout> layouts)
    {
        if (layouts == null)
            throw new ArgumentNullException(nameof(layouts));


        Debug.WriteLine("");
        Debug.WriteLine("========== CELL LAYOUT DEBUG ==========");


        foreach (var layout in layouts)
        {
            Debug.WriteLine(
                $"Cell:{layout.Cell.Name} | " +
                $"Row:{layout.RowIndex} | " +
                $"Col:{layout.ColumnIndex} | " +
                $"X:{layout.Left} | " +
                $"Y:{layout.Bottom} | " +
                $"Width:{layout.Width} | " +
                $"Height:{layout.Height} | " +
                $"Align:{layout.Cell.Style.HorizontalAlignment}/{layout.Cell.Style.VerticalAlignment}"
);


            Debug.WriteLine(
                $"  Style:{layout.Style}"
            );
        }


        Debug.WriteLine(
            "======================================");
    }
}