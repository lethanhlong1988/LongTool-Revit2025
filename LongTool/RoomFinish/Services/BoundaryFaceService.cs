using Autodesk.Revit.DB;

using LongTool.RoomFinish.Models;
using System.Diagnostics;

namespace LongTool.RoomFinish.Services;

public class BoundaryFaceService
{
    public Face? FindFace(
        BoundarySegmentData segment)
    {
        if (segment.Element == null)
            return null;


        if (segment.Element is not Wall wall)
            return null;


        return FindWallFace(
            wall,
            segment.Curve);
    }


    private Face? FindWallFace(
    Wall wall,
    Curve boundaryCurve)
    {
        Options options = new Options
        {
            ComputeReferences = true,
            DetailLevel = ViewDetailLevel.Fine
        };


        GeometryElement geometry =
            wall.get_Geometry(options);


        Face? bestFace = null;

        double minDistance = double.MaxValue;


        foreach (GeometryObject geometryObject in geometry)
        {
            if (geometryObject is not Solid solid)
                continue;


            foreach (Face face in solid.Faces)
            {
                XYZ point =
                    boundaryCurve.GetEndPoint(0);


                IntersectionResult? result =
                    face.Project(point);


                if (result == null)
                    continue;


                Debug.WriteLine(
                    $"Face Area : {face.Area}");

                Debug.WriteLine(
                    $"Distance : {result.Distance}");


                if (result.Distance < minDistance)
                {
                    minDistance = result.Distance;
                    bestFace = face;
                }
            }
        }


        return bestFace;
    }
}