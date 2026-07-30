using LongTool.Tables.Data;
using LongTool.Tables.Models;
using System;

namespace LongTool.Tables.Services;

public static class DoorScheduleService
{
    public static void Fill(Table table, DoorData door)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));

        if (door == null)
            throw new ArgumentNullException(nameof(door));


        var markCell = table.GetCell("Cell_A5");
        if (markCell != null)
        {
            markCell.Text = door.Mark;
        }


        var familyCell = table.GetCell("Cell_B5_C5_Merged");
        if (familyCell != null)
        {
            familyCell.Text = door.Family;
        }


        var widthCell = table.GetCell("Cell_A6");
        if (widthCell != null)
        {
            widthCell.Text = "Width";
        }


        var heightCell = table.GetCell("Cell_B6_C6_Merged");
        if (heightCell != null)
        {
            heightCell.Text = door.Height;
        }
    }
}