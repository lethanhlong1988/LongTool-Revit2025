using System.Collections.Generic;
using Autodesk.Revit.DB;
using LongTool.RoomFinish.Models;

namespace LongTool.RoomFinish.Renderers;

public class RoomBoundaryRenderer
{
    private readonly Document _document;
    private readonly View _view;

    public RoomBoundaryRenderer(Document document, View view)
    {
        _document = document;
        _view = view;
    }

    public void Draw(List<RoomData> rooms)
    {
        if (rooms == null) return;

        foreach (RoomData room in rooms)
        {
            Draw(room);
        }
    }

    public void Draw(RoomData room)
    {
        if (room == null) return;

        foreach (CurveLoop loop in room.BoundaryLoops)
        {
            foreach (Curve curve in loop)
            {
                _document.Create.NewDetailCurve(
                    _view,
                    curve);
            }
        }
    }
}
