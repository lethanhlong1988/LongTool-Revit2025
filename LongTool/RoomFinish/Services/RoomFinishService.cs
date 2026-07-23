using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using LongTool.RoomFinish.Models;
using LongTool.RoomFinish.Builders;

namespace LongTool.RoomFinish.Services;

/// <summary>
/// Facade Service - Orchestrates the entire room finish building process
/// </summary>
public class RoomFinishService
{
    private readonly RoomCollector _roomCollector;
    private readonly RoomBoundaryService _boundaryService;
    private readonly RoomSpatialService _spatialService;
    private readonly FinishSolidBuilder _finishSolidBuilder;

    public RoomFinishService(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        _roomCollector =
            new RoomCollector(document);

        _boundaryService =
            new RoomBoundaryService(document);

        _spatialService =
            new RoomSpatialService(document);

        _finishSolidBuilder =
            new FinishSolidBuilder();
    }

    /// <summary>
    /// Build complete room finish data for all rooms in the document
    /// </summary>
    /// <returns>List of RoomData with complete finish information</returns>
    public List<RoomData> Build()
    {
        // Step 1: Collect all rooms
        List<RoomData> rooms = _roomCollector.Collect();

        if (rooms.Count == 0)
        {
            return rooms;
        }

        // Step 2: Build complete room data
        foreach (RoomData room in rooms)
        {
            BuildRoomData(room);
        }

        return rooms;
    }

    /// <summary>
    /// Build complete data for a single room
    /// </summary>
    /// <param name="room">RoomData to build</param>
    public void BuildRoomData(RoomData room)
    {
        ArgumentNullException.ThrowIfNull(room);

        _boundaryService.Build(room);

        _spatialService.Build(room);

        FinishElementBuilder.Build(room);

        _finishSolidBuilder.Build(room);
    }
}