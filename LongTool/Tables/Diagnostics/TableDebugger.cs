using System;
using LongTool.Tables.Models;
using System.Diagnostics;

namespace LongTool.Tables.Diagnostics;

public static class TableDebugger
{
    public static void Show(Table table)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));


        Trace.WriteLine("");
        Trace.WriteLine("========== TABLE Trace ==========");
        Trace.WriteLine(table);


        int rowIndex = 0;


        foreach (var row in table.Rows)
        {
            Trace.WriteLine("");
            Trace.WriteLine($"Row {rowIndex} Height:{row.Height}");


            int cellIndex = 0;


            foreach (var cell in row)
            {
                Trace.WriteLine(
                    $"  Cell[{cellIndex}] " +
                    $"ID:{cell.Id} | " +
                    $"Name:{cell.Name ?? "(null)"} | " +
                    $"Text:\"{cell.Text}\" | " +
                    $"Span({cell.RowSpan},{cell.ColSpan})"
                );

                cellIndex++;
            }


            rowIndex++;
        }


        Trace.WriteLine("=================================");

        Trace.WriteLine("TABLE Trace FINISHED");
    }
}