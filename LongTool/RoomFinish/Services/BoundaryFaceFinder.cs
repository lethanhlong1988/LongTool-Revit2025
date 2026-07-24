using Autodesk.Revit.DB;
using LongTool.RoomFinish.Models;

namespace LongTool.RoomFinish.Services;

public class BoundaryFaceFinder
{
    private readonly Document _document;


    public BoundaryFaceFinder(Document document)
    {
        _document = document;
    }


    public Face? Find(BoundarySegmentData segment)
    {
        if (segment.Element == null)
            return null;


        GeometryElement? geometry =
            segment.Element.get_Geometry(
                new Options
                {
                    ComputeReferences = true,
                    DetailLevel = ViewDetailLevel.Fine
                });


        if (geometry == null)
            return null;


        foreach (GeometryObject geometryObject in geometry)
        {
            if (geometryObject is Solid solid)
            {
                Face? face =
                    FindFaceFromSolid(
                        solid,
                        segment.Curve);


                if (face != null)
                    return face;
            }
        }


        return null;
    }


    private Face? FindFaceFromSolid(
        Solid solid,
        Curve boundaryCurve)
    {
        foreach (Face face in solid.Faces)
        {
            if (face is not PlanarFace planarFace)
                continue;


            if (IsFaceNearCurve(
                    planarFace,
                    boundaryCurve))
            {
                return face;
            }
        }


        return null;
    }


    private bool IsFaceNearCurve(
        PlanarFace face,
        Curve curve)
    {
        XYZ start =
            curve.GetEndPoint(0);


        IntersectionResult? result =
            face.Project(start);


        if (result == null)
            return false;


        double distance =
            result.Distance;


        return distance < 0.01;
    }
}