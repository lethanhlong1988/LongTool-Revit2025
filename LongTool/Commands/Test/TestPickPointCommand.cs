using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.Core.Selection;
using LongTool.Core.Views;
using LongTool.Utils;
using System;

namespace LongTool.Commands.Test;


[Transaction(TransactionMode.Manual)]
public class TestPickPointCircleCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        UIDocument uidoc =
            commandData.Application.ActiveUIDocument;

        Document doc =
            uidoc.Document;


        try
        {
            // 1. Pick Origin
            PickPointResult? pick =
                PointPicker.PickPoint(
                    uidoc,
                    "Chọn vị trí tâm hình tròn");


            if (pick == null)
            {
                return Result.Cancelled;
            }


            XYZ origin =
                pick.Point;

            ViewCoordinateSystem cs =
                new ViewCoordinateSystem(
                    pick.View);


            TaskDialog.Show(
                "View Coordinate",
                cs.ToString());

            TaskDialog.Show(
                "Pick Info",
                $"Point:\n" +
                $"X = {pick.Point.X}\n" +
                $"Y = {pick.Point.Y}\n" +
                $"Z = {pick.Point.Z}\n\n" +

                $"View:\n" +
                $"{pick.View.Name}\n\n" +

                $"Type:\n" +
                $"{pick.ViewType}\n\n" +

                $"Direction:\n" +
                $"{pick.ViewDirection}\n\n" +

                $"Up:\n" +
                $"{pick.UpDirection}");



            // 2. Tạo Circle 100mm
            double radius =
                UnitUtilsEx.MmToInternal(50);


            Plane plane =
                Plane.CreateByNormalAndOrigin(
                    XYZ.BasisZ,
                    origin);


            Arc circle =
                Arc.Create(
                    plane,
                    radius,
                    0,
                    2 * Math.PI);


            // 3. Render Revit
            using Transaction trans =
                new Transaction(
                    doc,
                    "Draw Test Circle");


            trans.Start();


            doc.Create.NewModelCurve(
                circle,
                SketchPlane.Create(
                    doc,
                    plane));


            trans.Commit();


            return Result.Succeeded;
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
        {
            return Result.Cancelled;
        }
        catch (System.Exception ex)
        {
            message = ex.ToString();

            return Result.Failed;
        }
    }
}