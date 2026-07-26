using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using LongTool.Core.Storage;

namespace LongTool.Commands.Management;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class TestLongToolMarkerCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        UIDocument uiDocument = commandData.Application.ActiveUIDocument;
        Document document = uiDocument.Document;

        View activeView = document.ActiveView;

        if (activeView.ViewType != ViewType.FloorPlan &&
            activeView.ViewType != ViewType.CeilingPlan &&
            activeView.ViewType != ViewType.AreaPlan &&
            activeView.ViewType != ViewType.Section &&
            activeView.ViewType != ViewType.Elevation &&
            activeView.ViewType != ViewType.Detail &&
            activeView.ViewType != ViewType.DraftingView)
        {
            TaskDialog.Show(
                "LongTool",
                "Please run this command in a Plan, Section, Elevation, Detail or Drafting View.");

            return Result.Cancelled;
        }

        using Transaction transaction =
            new(document, "Test LongTool Marker");

        transaction.Start();

        Line line = Line.CreateBound(
            new XYZ(0, 0, 0),
            new XYZ(10, 0, 0));

        DetailCurve detailCurve =
            document.Create.NewDetailCurve(activeView, line);

        LongToolMarker.Mark(detailCurve);

        bool isMarked =
            LongToolMarker.IsMarked(detailCurve);

        transaction.Commit();

        TaskDialog.Show(
            "LongTool Marker",
            $"Element Id : {detailCurve.Id}\n" +
            $"IsMarked : {isMarked}");

        return Result.Succeeded;
    }
}