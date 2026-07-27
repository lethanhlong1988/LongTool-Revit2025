using Autodesk.Revit.DB;

using LongTool.RoomFinish.Models;
using LongTool.Core.Storage;

namespace LongTool.RoomFinish.Renderers;

public class FinishSolidRenderer
{
    private readonly Document _document;


    public FinishSolidRenderer(Document document)
    {
        _document = document;
    }


    public void Render(RoomData room)
    {
        foreach (FinishSolidData solidData in room.FinishSolids)
        {
            if (solidData.Solid == null)
            {
                continue;
            }


            DirectShape shape =
                DirectShape.CreateElement(
                    _document,
                    new ElementId(
                        BuiltInCategory.OST_GenericModel));


            shape.Name =
                "Room Finish Solid";


            shape.SetShape(
                new GeometryObject[]
                {
                    solidData.Solid
                });

            LongToolMarker.Mark(shape);
        }
    }
}