using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

using RevitGrid = Autodesk.Revit.DB.Grid;

namespace LongTool.Commands.JohAbroad
{
    [Transaction(TransactionMode.Manual)]
    public class AdjustGridTopToCropTopCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc =
                commandData.Application.ActiveUIDocument;

            if (uiDoc == null)
            {
                message = "Không có tài liệu Revit đang mở.";
                return Result.Failed;
            }

            Document doc = uiDoc.Document;

            try
            {
                // ============================================================
                // 1. Lấy các View đang được chọn trong Project Browser
                // ============================================================

                ICollection<ElementId> selectedIds =
                    uiDoc.Selection.GetElementIds();

                // ============================================================
                // 2. Chưa chọn gì
                // ============================================================

                if (selectedIds.Count == 0)
                {
                    TaskDialog.Show(
                        "Adjust Grid Top",
                        "Vui lòng chọn một hoặc nhiều mặt đứng / mặt cắt trong Project Browser."
                    );

                    return Result.Cancelled;
                }

                // ============================================================
                // 3. Lấy các View được chọn
                //
                // Chỉ xử lý:
                // - Elevation
                // - Section
                //
                // Các loại View khác sẽ bỏ qua.
                // ============================================================

                List<View> selectedViews = new();

                foreach (ElementId id in selectedIds)
                {
                    Element element = doc.GetElement(id);

                    if (element is not View view)
                        continue;

                    if (view.IsTemplate)
                        continue;

                    if (view.ViewType != ViewType.Elevation &&
                        view.ViewType != ViewType.Section)
                    {
                        continue;
                    }

                    if (!selectedViews.Contains(view))
                    {
                        selectedViews.Add(view);
                    }
                }

                // ============================================================
                // 4. Không có Elevation / Section
                // ============================================================

                if (selectedViews.Count == 0)
                {
                    TaskDialog.Show(
                        "Adjust Grid Top",
                        "Không có mặt đứng hoặc mặt cắt nào được chọn.\n\n" +
                        "Hãy chọn Elevation hoặc Section trong Project Browser."
                    );

                    return Result.Cancelled;
                }

                // ============================================================
                // 5. Thống kê
                // ============================================================

                int processedViews = 0;
                int skippedViews = 0;
                int foundGrids = 0;
                int modifiedGrids = 0;

                // ============================================================
                // 6. Transaction
                // ============================================================

                using Transaction transaction =
                    new Transaction(
                        doc,
                        "Adjust Grid Top To Crop Top");

                transaction.Start();

                // ============================================================
                // 7. Xử lý từng View
                // ============================================================

                foreach (View view in selectedViews)
                {
                    // --------------------------------------------------------
                    // View phải có Crop
                    // --------------------------------------------------------

                    if (!view.CropBoxActive)
                    {
                        skippedViews++;
                        continue;
                    }

                    BoundingBoxXYZ cropBox = view.CropBox;

                    if (cropBox == null)
                    {
                        skippedViews++;
                        continue;
                    }

                    processedViews++;

                    // --------------------------------------------------------
                    // Lấy Grid đang hiển thị trong View
                    // --------------------------------------------------------

                    List<RevitGrid> grids =
                        new FilteredElementCollector(doc, view.Id)
                            .OfClass(typeof(RevitGrid))
                            .Cast<RevitGrid>()
                            .ToList();

                    foundGrids += grids.Count;

                    // --------------------------------------------------------
                    // Xử lý từng Grid
                    // --------------------------------------------------------

                    foreach (RevitGrid grid in grids)
                    {
                        try
                        {
                            if (AdjustGridTop(grid, view))
                            {
                                modifiedGrids++;
                            }
                        }
                        catch
                        {
                            // Một Grid lỗi không làm dừng toàn bộ Command.
                            continue;
                        }
                    }
                }

                transaction.Commit();

                // ============================================================
                // 8. Làm mới View hiện tại
                // ============================================================

                uiDoc.RefreshActiveView();

                // ============================================================
                // 9. Kết quả
                // ============================================================

                TaskDialog.Show(
                    "Adjust Grid Top",
                    $"Hoàn thành.\n\n" +
                    $"View được chọn: {selectedViews.Count}\n" +
                    $"View đã xử lý: {processedViews}\n" +
                    $"View bỏ qua: {skippedViews}\n" +
                    $"Grid tìm thấy: {foundGrids}\n" +
                    $"Grid đã điều chỉnh: {modifiedGrids}"
                );

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;
            }
        }

        // ====================================================================
        // Điều chỉnh đầu trên Grid sát Crop Top
        // ====================================================================

        private static bool AdjustGridTop(
            RevitGrid grid,
            View view)
        {
            // ================================================================
            // 1. Lấy Curve của Grid trong View
            // ================================================================

            IList<Curve> curves =
                grid.GetCurvesInView(
                    DatumExtentType.ViewSpecific,
                    view);

            if (curves == null || curves.Count == 0)
                return false;

            // ================================================================
            // 2. Lấy CropBox
            // ================================================================

            BoundingBoxXYZ cropBox =
                view.CropBox;

            if (cropBox == null)
                return false;

            Transform cropTransform =
                cropBox.Transform;

            // ================================================================
            // 3. Crop Top
            //
            // Trong hệ tọa độ local của CropBox:
            //
            // Min.Y = đáy
            // Max.Y = đỉnh
            // ================================================================

            double cropTop =
                cropBox.Max.Y;

            bool modified = false;

            // ================================================================
            // 4. Xử lý từng Curve
            // ================================================================

            foreach (Curve curve in curves)
            {
                // ------------------------------------------------------------
                // Hiện tại chỉ xử lý Grid dạng Line
                // ------------------------------------------------------------

                if (curve is not Line line)
                    continue;

                XYZ p0 =
                    line.GetEndPoint(0);

                XYZ p1 =
                    line.GetEndPoint(1);

                // ------------------------------------------------------------
                // Đưa 2 đầu Grid vào hệ tọa độ CropBox
                // ------------------------------------------------------------

                XYZ localP0 =
                    cropTransform.Inverse.OfPoint(p0);

                XYZ localP1 =
                    cropTransform.Inverse.OfPoint(p1);

                // ------------------------------------------------------------
                // Xác định đầu dưới và đầu trên
                //
                // Local Y càng lớn = càng lên trên trong View.
                // ------------------------------------------------------------

                XYZ bottomLocal;
                XYZ topLocal;

                if (localP0.Y <= localP1.Y)
                {
                    bottomLocal = localP0;
                    topLocal = localP1;
                }
                else
                {
                    bottomLocal = localP1;
                    topLocal = localP0;
                }

                // ============================================================
                // 5. Chỉ xử lý khi đầu trên vượt Crop Top
                // ============================================================

                const double tolerance = 1e-7;

                if (topLocal.Y <= cropTop + tolerance)
                {
                    continue;
                }

                // ============================================================
                // 6. Vector từ đầu dưới đến đầu trên
                // ============================================================

                XYZ direction =
                    topLocal - bottomLocal;

                if (direction.GetLength() < 1e-9)
                    continue;

                // ============================================================
                // 7. Tìm vị trí giao giữa Grid và Crop Top
                //
                // P = Bottom + t * Direction
                //
                // Điều kiện:
                //
                // P.Y = CropTop
                // ============================================================

                if (Math.Abs(direction.Y) < 1e-9)
                    continue;

                double t =
                    (cropTop - bottomLocal.Y)
                    / direction.Y;

                // ------------------------------------------------------------
                // Với Grid vượt Crop Top, t phải nằm trong đoạn 0 → 1
                // ------------------------------------------------------------

                if (t <= 0 || t >= 1)
                    continue;

                // ============================================================
                // 8. Tạo đầu trên mới
                // ============================================================

                XYZ newTopLocal =
                    bottomLocal + direction * t;

                // Đảm bảo chính xác bằng Crop Top
                newTopLocal =
                    new XYZ(
                        newTopLocal.X,
                        cropTop,
                        newTopLocal.Z);

                // ============================================================
                // 9. Chuyển về Model Coordinate
                // ============================================================

                XYZ bottomModel =
                    cropTransform.OfPoint(bottomLocal);

                XYZ newTopModel =
                    cropTransform.OfPoint(newTopLocal);

                // ============================================================
                // 10. Tạo Line mới
                //
                // Đầu dưới giữ nguyên.
                // Đầu trên được kéo sát Crop Top.
                // ============================================================

                Line newLine =
                    Line.CreateBound(
                        bottomModel,
                        newTopModel);

                // ============================================================
                // 11. Gán Curve mới cho Grid trong View hiện tại
                // ============================================================

                grid.SetCurveInView(
                    DatumExtentType.ViewSpecific,
                    view,
                    newLine);

                modified = true;
            }

            return modified;
        }
    }
}
