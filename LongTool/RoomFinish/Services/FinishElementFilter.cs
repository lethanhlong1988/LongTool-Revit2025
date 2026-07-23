using Autodesk.Revit.DB;

namespace LongTool.RoomFinish.Services;

public static class FinishElementFilter
{
    public static bool IsFinishHost(Element element)
    {
        if (element.Category == null)
        {
            return false;
        }


        BuiltInCategory category =
            (BuiltInCategory)element.Category.Id.Value;


        return category == BuiltInCategory.OST_Walls
            || category == BuiltInCategory.OST_Columns;
    }
}