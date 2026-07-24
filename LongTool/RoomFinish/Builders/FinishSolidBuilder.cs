using System.Collections.Generic;
using System.Diagnostics;

using Autodesk.Revit.DB;

using LongTool.RoomFinish.Models;

namespace LongTool.RoomFinish.Builders;

public class FinishSolidBuilder
{
    public Solid Build(
        IList<CurveLoop> profile,
        XYZ direction,
        double thickness)
    {
        return GeometryCreationUtilities
            .CreateExtrusionGeometry(
                profile,
                direction,
                thickness);
    }


    public void Build(RoomData room)
    {
        FinishGeometryBuilder geometryBuilder =
            new FinishGeometryBuilder();


        double thickness =
            UnitUtils.ConvertToInternalUnits(
                20,
                UnitTypeId.Millimeters);



        foreach (FinishFaceData faceData in room.FinishFaces)
        {
            List<CurveLoop> profile =
                geometryBuilder.BuildProfile(faceData);



            Solid solid =
                Build(
                    profile,
                    -faceData.Normal,
                    thickness);

            Debug.WriteLine("------------------------------");

            Debug.WriteLine(
                $"Element Id : {faceData.HostElement.Id}");

            Debug.WriteLine(
                $"Face Area : {faceData.Area}");

            Debug.WriteLine(
                $"Normal : {faceData.Normal}");

            Debug.WriteLine(
                $"Thickness : {thickness}");



            if (solid == null)
            {
                Debug.WriteLine(
                    "Solid : NULL");

                continue;
            }

            Debug.WriteLine(
                $"Solid Volume : {solid.Volume}");



            FinishSolidData solidData =
                new FinishSolidData
                {
                    FaceData = faceData,

                    Solid = solid
                };


            room.FinishSolids.Add(solidData);
        }
    }
}