using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using LongTool.Infrastructure;

namespace LongTool.Commands.LongCommand;

// ✅ THÊM attribute này vào
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class Nut02 : CommandBase
{
    public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        ShowMessage("LongTool", "Đây là Nut 02!");
        return Result.Succeeded;
    }
}