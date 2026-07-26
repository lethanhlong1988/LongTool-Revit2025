using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using LongTool.Infrastructure;

namespace LongTool.Commands.Management;


// ✅ THÊM attribute này vào
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

public class ManegementButton : CommandBase
{
    public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        ShowMessage("LongTool", "Đây là Nut management 01!");
        return Result.Succeeded;
    }
}