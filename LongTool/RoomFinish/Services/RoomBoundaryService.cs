using System.Collections.Generic;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

using LongTool.RoomFinish.Models;

namespace LongTool.RoomFinish.Services;

/// <summary>
/// Chịu trách nhiệm lấy dữ liệu Boundary của Room
/// và bổ sung vào RoomData.
/// </summary>
public class RoomBoundaryService
{
    private readonly Document _document;

    public RoomBoundaryService(Document document)
    {
        _document = document;
    }

    /// <summary>
    /// Xây dựng BoundaryLoops cho RoomData.
    /// </summary>
    public void Build(RoomData roomData)
    {
        Room? room = _document.GetElement(roomData.Id) as Room;

        if (room == null)
            return;

        var options = new SpatialElementBoundaryOptions
        {
            SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish
        };

        IList<IList<BoundarySegment>>? boundarySegments =
            room.GetBoundarySegments(options);

        if (boundarySegments == null)
            return;

        roomData.BoundaryLoops.Clear();

        foreach (IList<BoundarySegment> boundaryLoop in boundarySegments)
        {
            CurveLoop curveLoop = new CurveLoop();

            foreach (BoundarySegment boundarySegment in boundaryLoop)
            {
                Curve curve = boundarySegment.GetCurve();

                curveLoop.Append(curve);
            }

            roomData.BoundaryLoops.Add(curveLoop);
        }


    }
}