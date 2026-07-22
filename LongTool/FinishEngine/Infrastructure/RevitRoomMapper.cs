using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using LongTool.FinishEngine.Domain;
using System;
using System.Linq;

namespace LongTool.FinishEngine.Infrastructure;

/// <summary>
/// Chuyển đổi Room của Revit thành FinishRoom.
/// </summary>
public class RevitRoomMapper
{
    public FinishRoom Map(Room room)
    {
        if (room == null)
            throw new ArgumentNullException(nameof(room));

        return new FinishRoom
        {
            Id = room.Id,
            Number = room.Number,
            Name = room.Name,
            LevelId = room.LevelId
        };
    }
}