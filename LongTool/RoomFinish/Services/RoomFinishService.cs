using Autodesk.Revit.DB;
using LongTool.RoomFinish.Builders;
using LongTool.RoomFinish.Models;
using LongTool.RoomFinish.Renderers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LongTool.RoomFinish.Services;

/// <summary>
/// Facade Service - Orchestrates the entire room finish building process
/// </summary>
public class RoomFinishService
{
    private readonly RoomCollector _roomCollector;

    private readonly RoomBoundaryService _boundaryService;

    private readonly BoundaryFaceService _boundaryFaceService;

    private readonly RoomSpatialService _spatialService;

    private readonly FinishSolidBuilder _finishSolidBuilder;

    private readonly BoundaryFaceFinder _faceFinder;

    private readonly FinishSolidRenderer _finishSolidRenderer;


    public RoomFinishService(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);


        _roomCollector =
            new RoomCollector(document);


        _boundaryService =
            new RoomBoundaryService(document);


        _boundaryFaceService =
            new BoundaryFaceService();


        _spatialService =
            new RoomSpatialService(document);


        _finishSolidBuilder =
            new FinishSolidBuilder();

        _faceFinder =
            new BoundaryFaceFinder(document);

        _finishSolidRenderer =
            new FinishSolidRenderer(document);
    }


    public List<RoomData> Build()
    {
        List<RoomData> rooms =
            _roomCollector.Collect();


        if (rooms.Count == 0)
        {
            return rooms;
        }


        foreach (RoomData room in rooms)
        {
            BuildRoomData(room);
        }


        return rooms;
    }


    public void BuildRoomData(RoomData room)
    {
        ArgumentNullException.ThrowIfNull(room);


        _boundaryService.Build(room);


        ProcessBoundaryFaces(room);


        _spatialService.Build(room);


        FinishElementBuilder.Build(room);


        _finishSolidBuilder.Build(room);

    }


    private void ProcessBoundaryFaces(RoomData room)
    {
        foreach (List<BoundarySegmentData> segments in room.BoundarySegments)
        {
            foreach (BoundarySegmentData segment in segments)
            {
                Face? face =
                    _boundaryFaceService.FindFace(segment);

                segment.Face = face;


                Debug.WriteLine(
                    $"Processed Face : {face != null}");
            }
        }
    }


}