using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using LongTool.Core.Management;

namespace LongTool.Commands.Management;

[Transaction(TransactionMode.Manual)]
public class SelectLongToolObjectsCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        UIDocument uiDocument = commandData.Application.ActiveUIDocument;

        int selectedCount = LongToolSelector.SelectAll(uiDocument);

        TaskDialog.Show(
            "Select LongTool Objects",
            $"Selected {selectedCount} LongTool object(s).");

        return Result.Succeeded;
    }
}