using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using LongTool.Core.Management;

namespace LongTool.Commands.Management;

[Transaction(TransactionMode.Manual)]
public class ClearLongToolObjectsCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        Document document = commandData.Application.ActiveUIDocument.Document;

        int deletedCount = LongToolCleaner.DeleteAll(document);

        TaskDialog.Show(
            "Clear LongTool Objects",
            $"Deleted {deletedCount} LongTool object(s).");

        return Result.Succeeded;
    }
}