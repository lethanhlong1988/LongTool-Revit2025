using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using LongTool.FinishEngine.Infrastructure;
using LongTool.Infrastructure;
using System.Linq;

namespace LongTool.Commands.Long;

/// <summary>
/// Command thử nghiệm Finish Engine.
/// </summary>
[Transaction(TransactionMode.ReadOnly)]
[Command("Finish Engine Test")]
public class FinishEngineTestCommand : CommandBase
{
    public override Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        Document document = commandData.Application.ActiveUIDocument.Document;

        var repository = new RevitRoomRepository();

        var rooms = repository.GetRooms(document);

        var revitRooms = new FilteredElementCollector(document)
            .OfCategory(BuiltInCategory.OST_Rooms)
            .WhereElementIsNotElementType()
            .Cast<Room>()
            .ToList();

        Room room = revitRooms.First();

        var options = new SpatialElementBoundaryOptions();

        var boundaries = room.GetBoundarySegments(options);

        BoundarySegment segment = boundaries[0][0];

        Curve curve = segment.GetCurve();

        var text = $"Found {rooms.Count} rooms.\n\n";

        foreach (var finishRoom in rooms)
        {
            text += $"{room.Number} - {room.Name}\n";
        }

        TaskDialog.Show("Finish Engine", text);

        return Result.Succeeded;
    }
}