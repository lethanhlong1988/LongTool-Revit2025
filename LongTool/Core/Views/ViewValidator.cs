using Autodesk.Revit.DB;

namespace LongTool.Core.Views;

public static class ViewValidator
{
    public static bool Is2DWorkingView(View view)
    {
        if (view == null)
            return false;

        return
            view.ViewType == ViewType.FloorPlan ||
            view.ViewType == ViewType.CeilingPlan ||
            view.ViewType == ViewType.DraftingView ||
            view.ViewType == ViewType.Legend;
    }
}
