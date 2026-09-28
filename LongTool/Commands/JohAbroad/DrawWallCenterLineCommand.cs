using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using LongTool.Services.LineStyle;

namespace LongTool.Commands.JohAbroad
{
    [Transaction(TransactionMode.Manual)]
    public class DrawWallCenterLineCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            View view = doc.ActiveView;

            // ============================================================
            // 1. LẤY CÁC WALL ĐANG ĐƯỢC CHỌN
            // ============================================================

            ICollection<ElementId> selectedIds =
                uidoc.Selection.GetElementIds();

            if (selectedIds.Count == 0)
            {
                TaskDialog.Show(
                    "Draw Wall Center Line",
                    "Vui lòng chọn ít nhất một Wall trước khi chạy lệnh.");

                return Result.Cancelled;
            }

            List<Wall> walls = selectedIds
                .Select(id => doc.GetElement(id))
                .OfType<Wall>()
                .ToList();

            if (walls.Count == 0)
            {
                TaskDialog.Show(
                    "Draw Wall Center Line",
                    "Không có Wall nào được chọn.");

                return Result.Cancelled;
            }

            // ============================================================
            // CHỌN LINE STYLE
            // ============================================================

            LineStyleSelectorService lineStyleSelector =
                new LineStyleSelectorService();

            GraphicsStyle selectedLineStyle =
                lineStyleSelector.Select(doc);

            if (selectedLineStyle == null)
            {
                return Result.Cancelled;
            }

            // ============================================================
            // 2. KIỂM TRA VIEW
            // ============================================================

            bool isPlanView =
                view.ViewType == ViewType.FloorPlan ||
                view.ViewType == ViewType.CeilingPlan ||
                view.ViewType == ViewType.EngineeringPlan;

            bool isVerticalView =
                view.ViewType == ViewType.Elevation ||
                view.ViewType == ViewType.Section;

            if (!isPlanView && !isVerticalView)
            {
                TaskDialog.Show(
                    "Draw Wall Center Line",
                    "View hiện tại không được hỗ trợ.\n\n" +
                    "Hỗ trợ:\n" +
                    "• Floor Plan\n" +
                    "• Ceiling Plan\n" +
                    "• Engineering Plan\n" +
                    "• Elevation\n" +
                    "• Section");

                return Result.Cancelled;
            }

            // ============================================================
            // 3. VẼ TIM
            // ============================================================

            int createdCount = 0;
            int skippedCount = 0;

            using (Transaction trans = new Transaction(
                doc,
                "Draw Wall Center Lines"))
            {
                trans.Start();

                foreach (Wall wall in walls)
                {
                    try
                    {
                        DetailCurve detailCurve = null;

                        if (isPlanView)
                        {
                            detailCurve =
                                CreateCenterLineOnPlan(
                                    doc,
                                    view,
                                    wall);
                        }
                        else if (isVerticalView)
                        {
                            detailCurve =
                                CreateCenterLineOnVerticalView(
                                    doc,
                                    view,
                                    wall);
                        }

                        if (detailCurve != null)
                        {
                            // ========================================================
                            // GÁN LINE STYLE ĐÃ CHỌN
                            // ========================================================

                            detailCurve.LineStyle =
                                selectedLineStyle;

                            createdCount++;
                        }
                        else
                        {
                            skippedCount++;
                        }
                    }
                    catch
                    {
                        skippedCount++;
                    }
                }

                trans.Commit();
            }

            // ============================================================
            // 4. THÔNG BÁO KẾT QUẢ
            // ============================================================

            string resultMessage =
                $"Đã chọn: {walls.Count} Wall\n" +
                $"Đã vẽ tim: {createdCount}";

            if (skippedCount > 0)
            {
                resultMessage +=
                    $"\nKhông thể vẽ: {skippedCount}";
            }

            TaskDialog.Show(
                "Draw Wall Center Line",
                resultMessage);

            return Result.Succeeded;
        }

        // ================================================================
        // MẶT BẰNG
        // ================================================================

        private DetailCurve CreateCenterLineOnPlan(
            Document doc,
            View view,
            Wall wall)
        {
            LocationCurve locationCurve =
                wall.Location as LocationCurve;

            if (locationCurve == null)
                return null;

            Curve wallCurve =
                locationCurve.Curve;

            if (wallCurve == null)
                return null;

            if (!wallCurve.IsBound)
                return null;

            // Location Curve của Wall chính là đường tim.
            DetailCurve detailCurve =
                doc.Create.NewDetailCurve(
                    view,
                    wallCurve);

            return detailCurve;
        }

        // ================================================================
        // MẶT ĐỨNG / MẶT CẮT
        // ================================================================

        private DetailCurve CreateCenterLineOnVerticalView(
            Document doc,
            View view,
            Wall wall)
        {
            LocationCurve locationCurve =
                wall.Location as LocationCurve;

            if (locationCurve == null)
                return null;

            Curve wallCurve =
                locationCurve.Curve;

            if (wallCurve == null)
                return null;

            if (!wallCurve.IsBound)
                return null;

            // ============================================================
            // 1. HƯỚNG CỦA WALL
            // ============================================================

            XYZ wallDirection =
                GetCurveDirection(wallCurve);

            if (wallDirection == null)
                return null;

            // ============================================================
            // 2. HƯỚNG NHÌN CỦA VIEW
            // ============================================================

            XYZ viewDirection =
                view.ViewDirection.Normalize();

            // ============================================================
            // 3. KIỂM TRA WALL CÓ VUÔNG GÓC VỚI VIEW KHÔNG
            // ============================================================

            double dot =
                Math.Abs(
                    wallDirection.DotProduct(viewDirection));

            /*
             * Nếu Wall vuông góc với hướng nhìn:
             *
             * WallDirection · ViewDirection ≈ 1
             *
             * Nếu Wall song song với hướng nhìn:
             *
             * WallDirection · ViewDirection ≈ 0
             */

            const double tolerance = 0.15;

            if (dot < 1.0 - tolerance)
            {
                return null;
            }

            // ============================================================
            // 4. LẤY TÂM WALL
            // ============================================================

            XYZ wallCenter =
                wallCurve.Evaluate(
                    0.5,
                    true);

            // ============================================================
            // 5. LẤY BOUNDING BOX CỦA WALL TRONG VIEW
            // ============================================================

            BoundingBoxXYZ bbox =
                wall.get_BoundingBox(view);

            if (bbox == null)
            {
                bbox =
                    wall.get_BoundingBox(null);
            }

            if (bbox == null)
                return null;

            // ============================================================
            // 6. CHIẾU TÂM WALL LÊN MẶT PHẲNG VIEW
            // ============================================================

            XYZ projectedCenter =
                ProjectPointToViewPlane(
                    wallCenter,
                    view);

            // ============================================================
            // 7. LẤY VIEW UP DIRECTION
            // ============================================================

            XYZ up =
                view.UpDirection.Normalize();

            XYZ right =
                view.RightDirection.Normalize();

            // ============================================================
            // 8. TÌM GIỚI HẠN TRÊN / DƯỚI CỦA WALL
            // ============================================================

            List<XYZ> corners =
                GetBoundingBoxCorners(bbox);

            double minUp = double.MaxValue;
            double maxUp = double.MinValue;

            foreach (XYZ corner in corners)
            {
                XYZ projected =
                    ProjectPointToViewPlane(
                        corner,
                        view);

                double upValue =
                    projected.DotProduct(up);

                minUp =
                    Math.Min(
                        minUp,
                        upValue);

                maxUp =
                    Math.Max(
                        maxUp,
                        upValue);
            }

            if (Math.Abs(maxUp - minUp) < 0.001)
                return null;

            // ============================================================
            // 9. TẠO ĐIỂM ĐẦU / CUỐI CỦA TIM TƯỜNG
            // ============================================================

            double centerRight =
                projectedCenter.DotProduct(right);

            XYZ startPoint =
                CreatePointFromViewCoordinates(
                    projectedCenter,
                    right,
                    up,
                    centerRight,
                    minUp);

            XYZ endPoint =
                CreatePointFromViewCoordinates(
                    projectedCenter,
                    right,
                    up,
                    centerRight,
                    maxUp);

            // ============================================================
            // 10. TẠO DETAIL LINE
            // ============================================================

            Line centerLine =
                Line.CreateBound(
                    startPoint,
                    endPoint);

            DetailCurve detailCurve =
                doc.Create.NewDetailCurve(
                    view,
                    centerLine);

            return detailCurve;
        }

        // ================================================================
        // LẤY HƯỚNG CỦA CURVE
        // ================================================================

        private XYZ GetCurveDirection(Curve curve)
        {
            if (curve is Line line)
            {
                return line.Direction.Normalize();
            }

            XYZ start =
                curve.GetEndPoint(0);

            XYZ end =
                curve.GetEndPoint(1);

            XYZ direction =
                end - start;

            if (direction.GetLength() < 0.000001)
                return null;

            return direction.Normalize();
        }

        // ================================================================
        // CHIẾU ĐIỂM LÊN VIEW PLANE
        // ================================================================

        private XYZ ProjectPointToViewPlane(
            XYZ point,
            View view)
        {
            XYZ origin =
                view.Origin;

            XYZ normal =
                view.ViewDirection.Normalize();

            XYZ vector =
                point - origin;

            double distance =
                vector.DotProduct(normal);

            return point -
                   normal.Multiply(distance);
        }

        // ================================================================
        // TẠO POINT THEO HỆ TỌA ĐỘ VIEW
        // ================================================================

        private XYZ CreatePointFromViewCoordinates(
            XYZ referencePoint,
            XYZ right,
            XYZ up,
            double rightValue,
            double upValue)
        {
            double currentRight =
                referencePoint.DotProduct(right);

            double currentUp =
                referencePoint.DotProduct(up);

            XYZ point =
                referencePoint;

            point +=
                right.Multiply(
                    rightValue - currentRight);

            point +=
                up.Multiply(
                    upValue - currentUp);

            return point;
        }

        // ================================================================
        // LẤY 8 GÓC CỦA BOUNDING BOX
        // ================================================================

        private List<XYZ> GetBoundingBoxCorners(
            BoundingBoxXYZ bbox)
        {
            List<XYZ> points =
                new List<XYZ>();

            XYZ min = bbox.Min;
            XYZ max = bbox.Max;

            Transform transform =
                bbox.Transform;

            points.Add(
                transform.OfPoint(
                    new XYZ(
                        min.X,
                        min.Y,
                        min.Z)));

            points.Add(
                transform.OfPoint(
                    new XYZ(
                        max.X,
                        min.Y,
                        min.Z)));

            points.Add(
                transform.OfPoint(
                    new XYZ(
                        min.X,
                        max.Y,
                        min.Z)));

            points.Add(
                transform.OfPoint(
                    new XYZ(
                        max.X,
                        max.Y,
                        min.Z)));

            points.Add(
                transform.OfPoint(
                    new XYZ(
                        min.X,
                        min.Y,
                        max.Z)));

            points.Add(
                transform.OfPoint(
                    new XYZ(
                        max.X,
                        min.Y,
                        max.Z)));

            points.Add(
                transform.OfPoint(
                    new XYZ(
                        min.X,
                        max.Y,
                        max.Z)));

            points.Add(
                transform.OfPoint(
                    new XYZ(
                        max.X,
                        max.Y,
                        max.Z)));

            return points;
        }
    }
}
