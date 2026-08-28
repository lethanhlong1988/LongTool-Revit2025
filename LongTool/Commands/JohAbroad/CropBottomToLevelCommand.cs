using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LongTool.Commands.JohAbroad
{
    [Transaction(TransactionMode.Manual)]
    public class CropBottomToLevelCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // =====================================================
            // 1. Lấy các View đang được chọn trong Project Browser
            // =====================================================

            ICollection<ElementId> selectedIds =
                uidoc.Selection.GetElementIds();

            if (selectedIds.Count == 0)
            {
                TaskDialog.Show(
                    "Crop Bottom to Level",
                    "Vui lòng chọn một hoặc nhiều Elevation hoặc Section trong Project Browser.");

                return Result.Cancelled;
            }

            // =====================================================
            // 2. Chỉ lấy Elevation và Section
            // =====================================================

            List<View> targetViews = selectedIds
                .Select(id => doc.GetElement(id))
                .OfType<View>()
                .Where(view =>
                    view.ViewType == ViewType.Elevation ||
                    view.ViewType == ViewType.Section)
                .ToList();

            if (targetViews.Count == 0)
            {
                TaskDialog.Show(
                    "Crop Bottom to Level",
                    "Không có Elevation hoặc Section View nào được chọn.");

                return Result.Cancelled;
            }

            // =====================================================
            // 3. Lấy tất cả Level trong Model
            // =====================================================

            List<Level> levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .OrderBy(level => level.Elevation)
                .ToList();

            if (levels.Count == 0)
            {
                TaskDialog.Show(
                    "Crop Bottom to Level",
                    "Không tìm thấy Level nào trong Model.");

                return Result.Succeeded;
            }

            int processedViewCount = 0;
            int adjustedViewCount = 0;

            using (Transaction trans =
                   new Transaction(doc, "Crop Bottom to Level"))
            {
                trans.Start();

                foreach (View view in targetViews)
                {
                    processedViewCount++;

                    // =================================================
                    // 4. Kiểm tra Crop Region
                    // =================================================

                    if (!view.CropBoxActive)
                        continue;

                    BoundingBoxXYZ cropBox = view.CropBox;

                    if (cropBox == null)
                        continue;

                    // =================================================
                    // 5. Lấy điểm giữa của mép dưới Crop Region
                    // =================================================

                    Transform transform = cropBox.Transform;

                    XYZ cropBottomPointLocal = new XYZ(
                        0,
                        cropBox.Min.Y,
                        0);

                    // Chuyển từ CropBox Local → Model Coordinates
                    XYZ cropBottomPointModel =
                        transform.OfPoint(cropBottomPointLocal);

                    double currentCropBottomElevation =
                        cropBottomPointModel.Z;

                    // =================================================
                    // 6. Tìm Level gần nhất nằm PHÍA TRÊN
                    //    Crop Bottom hiện tại
                    // =================================================

                    Level targetLevel = levels
                        .Where(level =>
                            level.Elevation > currentCropBottomElevation)
                        .OrderBy(level =>
                            level.Elevation)
                        .FirstOrDefault();

                    // Không có Level nào phía trên
                    if (targetLevel == null)
                        continue;

                    // =================================================
                    // 7. Chuyển cao độ Level từ Model Coordinates
                    //    sang CropBox Local Coordinates
                    // =================================================

                    XYZ levelPointModel = new XYZ(
                        cropBottomPointModel.X,
                        cropBottomPointModel.Y,
                        targetLevel.Elevation);

                    XYZ levelPointLocal =
                        transform.Inverse.OfPoint(levelPointModel);

                    double newCropBottomY =
                        levelPointLocal.Y;

                    // =================================================
                    // 8. Chỉ cho phép Crop Bottom đi LÊN
                    //
                    // newCropBottomY > old Min.Y
                    //
                    // Điều này đảm bảo Crop Region bị thu hẹp
                    // từ phía dưới lên.
                    // =================================================

                    if (newCropBottomY <= cropBox.Min.Y)
                        continue;

                    // =================================================
                    // 9. Chỉ thay đổi Crop Bottom
                    //
                    // Min.X  → giữ nguyên
                    // Min.Y  → thay đổi
                    // Min.Z  → giữ nguyên
                    //
                    // Max.X/Y/Z → giữ nguyên
                    // =================================================

                    XYZ oldMin = cropBox.Min;

                    cropBox.Min = new XYZ(
                        oldMin.X,
                        newCropBottomY,
                        oldMin.Z);

                    // =================================================
                    // 10. Gán CropBox trở lại View
                    // =================================================

                    view.CropBox = cropBox;

                    adjustedViewCount++;
                }

                trans.Commit();
            }

            // =====================================================
            // 11. Thông báo kết quả
            // =====================================================

            if (adjustedViewCount == 0)
            {
                TaskDialog.Show(
                    "Crop Bottom to Level",
                    $"Đã kiểm tra {processedViewCount} View.\n\n" +
                    "Không có View nào cần điều chỉnh Crop Bottom.");
            }
            else
            {
                TaskDialog.Show(
                    "Crop Bottom to Level",
                    $"Đã align Crop Bottom vào Level phía trên.\n\n" +
                    $"Views đã điều chỉnh: {adjustedViewCount}/{processedViewCount}");
            }

            return Result.Succeeded;
        }
    }
}
