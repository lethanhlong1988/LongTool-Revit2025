using System.Collections.Generic;
using System.Diagnostics;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

using LongTool.RoomFinish.Models;

namespace LongTool.RoomFinish.Services;

public class RoomSpatialService
{
    private readonly Document _document;

    private readonly SpatialElementGeometryCalculator _calculator;


    public RoomSpatialService(Document document)
    {
        _document = document;

        _calculator =
            new SpatialElementGeometryCalculator(document);
    }


    /// <summary>
    /// Phân tích không gian của Room.
    /// </summary>
    public void Build(RoomData roomData)
    {
        Room? room =
            _document.GetElement(roomData.Id) as Room;


        roomData.FinishFaces.Clear();
        roomData.FinishElements.Clear();
        roomData.FinishSolids.Clear();

        if (room == null)
        {
            return;
        }


        SpatialElementGeometryResults results =
            _calculator.CalculateSpatialElementGeometry(room);


        Solid solid =
            results.GetGeometry();


        foreach (Face face in solid.Faces)
        {
            IList<SpatialElementBoundarySubface> subfaces =
                results.GetBoundaryFaceInfo(face);


            foreach (SpatialElementBoundarySubface subface in subfaces)
            {
                ElementId elementId =
                    subface.SpatialBoundaryElement
                           .HostElementId;


                Element? element =
                     _document.GetElement(elementId);


                if (element == null)
                {
                    continue;
                }


                if (!FinishElementFilter.IsFinishHost(element))
                {
                    continue;
                }

                BoundingBoxUV box =
                    face.GetBoundingBox();

                UV uvCenter =
                    new UV(
                        (box.Min.U + box.Max.U) / 2.0,
                        (box.Min.V + box.Max.V) / 2.0);

                XYZ center =
                    face.Evaluate(uvCenter);

                XYZ normal =
                    face.ComputeNormal(uvCenter);

                FinishFaceData finishFace =
                    new FinishFaceData
                    {
                        HostElement = element,

                        Face = face,

                        BoundarySubface = subface,

                        Area = face.Area,

                        Center = center,

                        Normal = normal
                    };

                roomData.FinishFaces.Add(finishFace);
            }
        }


        Debug.WriteLine("==============================");
        Debug.WriteLine(
            $"Room : {roomData.Number}");

        Debug.WriteLine(
            $"Finish Face Count : {roomData.FinishFaces.Count}");
        Debug.WriteLine("==============================");


        foreach (FinishFaceData finishFace in roomData.FinishFaces)
        {
            Debug.WriteLine("------------------------------");

            Debug.WriteLine(
                $"Area : {finishFace.Area}");

            Debug.WriteLine(
                $"Category : {finishFace.HostElement.Category?.Name}");

            Debug.WriteLine(
                $"Element Id : {finishFace.HostElement.Id}");
        }
    }
}