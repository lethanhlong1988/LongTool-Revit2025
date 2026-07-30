using System;
using System.Collections.Generic;
using System.Diagnostics;
using LongTool.Tables.Models;

namespace LongTool.Tables.Diagnostics;

public static class LayoutBorderDebugger
{
    public static void Show(
        List<CellLayout> layouts)
    {
        if (layouts == null)
            throw new ArgumentNullException(nameof(layouts));


        Debug.WriteLine("");
        Debug.WriteLine("========== LAYOUT BORDER DEBUG ==========");


        foreach (var layout in layouts)
        {
            double x1 = layout.X;
            double y1 = layout.Y;

            double x2 = layout.X + layout.Width;
            double y2 = layout.Y + layout.Height;


            Debug.WriteLine(
                $"Cell:{layout.Cell.Name}");

            Debug.WriteLine(
                $"  Top    : ({x1},{y1}) -> ({x2},{y1})");

            Debug.WriteLine(
                $"  Bottom : ({x1},{y2}) -> ({x2},{y2})");

            Debug.WriteLine(
                $"  Left   : ({x1},{y1}) -> ({x1},{y2})");

            Debug.WriteLine(
                $"  Right  : ({x2},{y1}) -> ({x2},{y2})");

            Debug.WriteLine("");
        }


        Debug.WriteLine(
            "========================================");
    }
}