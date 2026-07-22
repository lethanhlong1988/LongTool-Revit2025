using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using LongTool.RoomFinish.Models;
using System.Collections.Generic;
using System.Linq;

namespace LongTool.RoomFinish.Services;

public class RoomCollector
{
    private readonly Document _document;

    public RoomCollector(Document document)
    {
        _document = document;
    }

    public List<RoomData> Collect()
    {
        return new FilteredElementCollector(_document)
            .OfCategory(BuiltInCategory.OST_Rooms)
            .WhereElementIsNotElementType()
            .OfType<Room>()
            .Where(IsValidRoom)
            .Select(room => new RoomData
            {
                Id = room.Id,
                Number = room.Number,
                Name = room.Name,
                Area = room.Area,
                LevelId = room.LevelId,
                LevelName = _document.GetElement(room.LevelId)?.Name ?? string.Empty
            })
            .ToList();
    }

    private static bool IsValidRoom(Room room)
    {
        return room.Area > 0 &&
               room.LevelId != ElementId.InvalidElementId;
    }
}