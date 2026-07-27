using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using LongTool.Infrastructure;

namespace LongTool.Commands.Table;

// ✅ THÊM attribute này vào
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

public class DrawTableCommand : CommandBase
{
    public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        ShowMessage("LongTool", "Coming Soon...");
        return Result.Succeeded;
    }
}