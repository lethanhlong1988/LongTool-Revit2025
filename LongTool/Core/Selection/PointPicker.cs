using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using LongTool.Core.Views;
using System;

namespace LongTool.Core.Selection;

public static class PointPicker
{
    public static PickPointResult? PickPoint(
    UIDocument uidoc,
    string message = "Chọn vị trí gốc")
    {
        View view =
            uidoc.Document.ActiveView;


        if (!ViewValidator.Is2DWorkingView(view))
        {
            TaskDialog.Show(
                "LongTool",
                "Môi trường hiện tại không phù hợp.\n\n" +
                "Vui lòng sử dụng:\n" +
                "- Floor Plan\n" +
                "- Drafting View");

            return null;
        }


        XYZ point =
            uidoc.Selection.PickPoint(
                ObjectSnapTypes.Endpoints |
                ObjectSnapTypes.Intersections |
                ObjectSnapTypes.Nearest,
                message);


        return new PickPointResult(
            point,
            view);
    }
}