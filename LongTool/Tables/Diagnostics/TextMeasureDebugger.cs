using Autodesk.Revit.DB;
using System.Diagnostics;

namespace LongTool.Tables.Diagnostics;

public static class TextMeasureDebugger
{
    public static void Show(
        TextNote note,
        View view,
        string cellName)
    {
        if (note == null)
            return;

        BoundingBoxXYZ box =
            note.get_BoundingBox(view);

        if (box == null)
            return;

        double width =
            box.Max.X - box.Min.X;

        double height =
            box.Max.Y - box.Min.Y;

        Debug.WriteLine(
            $"[{cellName}] " +
            $"Text Width = {width:F4} ft, " +
            $"Height = {height:F4} ft");
    }
}