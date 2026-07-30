using System;
using LongTool.Tables.Models;
using LongTool.Tables.Models.Styles;

namespace LongTool.Tables.Services;

public static class TableStyleResolver
{
    public static TableCellStyle Resolve(
        Table table,
        TableCell cell)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));


        if (cell == null)
            throw new ArgumentNullException(nameof(cell));


        if (cell.HasStyle)
        {
            return cell.Style!;
        }


        return table.DefaultStyle.CellStyle;
    }
}