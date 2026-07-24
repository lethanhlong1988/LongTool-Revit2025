using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using LongTool.Infrastructure;

namespace LongTool.Commands.Basic;

public class CommandHello : CommandBase
{
    public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        ShowMessage("LongTool", "Xin chào! Bạn đã nhấn vào nút Lời chào.");
        return Result.Succeeded;
    }
}