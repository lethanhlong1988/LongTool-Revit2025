using System;
using System.Text;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using LongTool.Core.Management;

namespace LongTool.Commands.Management;

[Transaction(TransactionMode.Manual)]
public class TestLongToolCollectorCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        Document document = commandData.Application.ActiveUIDocument.Document;

        var elementIds = LongToolElementCollector.Collect(document);

        var builder = new StringBuilder();

        builder.AppendLine($"Count : {elementIds.Count}");
        builder.AppendLine();

        foreach (ElementId id in elementIds)
        {
            builder.AppendLine(id.Value.ToString());
        }

        TaskDialog.Show(
            "LongTool Objects",
            builder.ToString());

        return Result.Succeeded;
    }
}