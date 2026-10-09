using Autodesk.Revit.UI;

namespace LongTool.RevitAdapters.Ui;

internal static class RevitTaskDialog
{
    public static void ShowInfo(string title, string message)
    {
        TaskDialog.Show(title, message);
    }

    public static bool Confirm(string title, string message)
    {
        var result = TaskDialog.Show(
            title,
            message,
            TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No);

        return result == TaskDialogResult.Yes;
    }
}