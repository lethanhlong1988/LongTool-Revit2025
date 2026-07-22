using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using LongTool.Infrastructure;

namespace LongTool.Commands.Advanced
{
    public class CommandAdvanced : CommandBase
    {
        public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ShowMessage("LongTool", "Đây là lệnh nâng cao!");
            return Result.Succeeded;
        }
    }
}