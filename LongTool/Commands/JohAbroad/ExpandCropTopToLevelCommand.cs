using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LongTool.Commands.JohAbroad
{
    [Transaction(TransactionMode.Manual)]
    public class ExpandCropTopToLevelCommand : IExternalCommand
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
                    "Expand Crop Top",
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
                    "Expand Crop Top",
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
                    "Expand Crop Top",
                    "Không tìm thấy Level nào trong Model.");

                return Result.Succeeded;
            }

            int processedViewCount = 0;
            int expandedViewCount = 0;

            using (Transaction trans =
                   new Transaction(doc, "Expand Crop Top To Level"))
            {
                trans.Start();

                foreach (View view in targetViews)
                {
                    processedViewCount++;

                    // -------------------------------------------------
                    // View phải đang sử dụng Crop Region
                    // -------------------------------------------------

                    if (!view.CropBoxActive)
                        continue;

                    BoundingBoxXYZ cropBox = view.CropBox;

                    if (cropBox == null)
                        continue;

                    // -------------------------------------------------
                    // Transform của CropBox:
                    //
                    // CropBox local coordinates:
                    // X = ngang màn hình
                    // Y = lên / xuống
                    // Z = hướng nhìn
                    //
                    // Chúng ta chỉ cần mép trên => local Y
                    // -------------------------------------------------

                    Transform transform = cropBox.Transform;

                    // Điểm giữa của mép trên CropBox
                    XYZ cropTopPointLocal = new XYZ(
                        0,
                        cropBox.Max.Y,
                        0);

                    // Chuyển sang Model Coordinates
                    XYZ cropTopPointModel =
                        transform.OfPoint(cropTopPointLocal);

                    double currentCropTopElevation =
                        cropTopPointModel.Z;

                    // -------------------------------------------------
                    // Tìm Level đầu tiên nằm phía trên Crop Top
                    // -------------------------------------------------

                    Level targetLevel = levels
                        .Where(level =>
                            level.Elevation > currentCropTopElevation)
                        .OrderBy(level =>
                            level.Elevation)
                        .FirstOrDefault();

                    // Không có Level nào phía trên
                    if (targetLevel == null)
                        continue;

                    // -------------------------------------------------
                    // Chuyển cao độ Level từ Model Coordinates
                    // sang CropBox Local Coordinates
                    // -------------------------------------------------

                    XYZ levelPointModel = new XYZ(
                        cropTopPointModel.X,
                        cropTopPointModel.Y,
                        targetLevel.Elevation);

                    XYZ levelPointLocal =
                        transform.Inverse.OfPoint(levelPointModel);

                    double newCropTopY =
                        levelPointLocal.Y;

                    // -------------------------------------------------
                    // Đảm bảo Crop Top thực sự được mở rộng
                    // -------------------------------------------------

                    if (newCropTopY <= cropBox.Max.Y)
                        continue;

                    // -------------------------------------------------
                    // Chỉ thay đổi mép trên
                    // -------------------------------------------------

                    XYZ oldMax = cropBox.Max;

                    cropBox.Max = new XYZ(
                        oldMax.X,
                        newCropTopY,
                        oldMax.Z);

                    // -------------------------------------------------
                    // Gán CropBox trở lại View
                    // -------------------------------------------------

                    view.CropBox = cropBox;

                    expandedViewCount++;
                }

                trans.Commit();
            }

            // =====================================================
            // 4. Thông báo kết quả
            // =====================================================

            if (expandedViewCount == 0)
            {
                TaskDialog.Show(
                    "Expand Crop Top",
                    $"Đã kiểm tra {processedViewCount} View.\n\n" +
                    "Không có View nào cần mở rộng Crop Region.");
            }
            else
            {
                TaskDialog.Show(
                    "Expand Crop Top",
                    $"Đã mở rộng mép trên Crop Region.\n\n" +
                    $"Views đã xử lý: {expandedViewCount}/{processedViewCount}");
            }

            return Result.Succeeded;
        }
    }
}

