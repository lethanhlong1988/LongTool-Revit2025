using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace LongTool.Commands.Basic
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class CommandSetting : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Hiển thị hộp thoại thông báo Revit-style [citation:2]
            TaskDialog.Show("LongTool", "Bạn đã nhấn vào nút Setting.");

            return Result.Succeeded;
        }
    }
}