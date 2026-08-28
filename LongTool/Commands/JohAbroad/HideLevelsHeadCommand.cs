
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LongTool.Commands.JohAbroad
{
    [Transaction(TransactionMode.Manual)]
    public class HideLevelsHeadCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // =====================================================
            // 1. Lấy các View đang được chọn
            // =====================================================

            ICollection<ElementId> selectedIds =
                uidoc.Selection.GetElementIds();

            if (selectedIds.Count == 0)
            {
                TaskDialog.Show(
                    "Hide Level Heads",
                    "Vui lòng chọn một hoặc nhiều Elevation hoặc Section trong Project Browser.");

                return Result.Cancelled;
            }

            // =====================================================
            // 2. Lọc chỉ Elevation và Section View
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
                    "Hide Level Heads",
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
                .ToList();

            if (levels.Count == 0)
            {
                TaskDialog.Show(
                    "Hide Level Heads",
                    "Không tìm thấy Level nào trong Model.");

                return Result.Succeeded;
            }

            // =====================================================
            // 4. Thực hiện Hide Level Heads
            // =====================================================

            int processedViewCount = 0;
            int hiddenBubbleCount = 0;

            using (Transaction trans =
                   new Transaction(doc, "Hide Level Heads"))
            {
                trans.Start();

                foreach (View view in targetViews)
                {
                    bool viewProcessed = false;

                    foreach (Level level in levels)
                    {
                        // -------------------------------------------------
                        // Level phải có khả năng hiển thị trong View này
                        // -------------------------------------------------

                        if (!level.CanBeVisibleInView(view))
                            continue;

                        // =================================================
                        // End 0
                        // =================================================

                        try
                        {
                            if (level.IsBubbleVisibleInView(
                                    DatumEnds.End0,
                                    view))
                            {
                                level.HideBubbleInView(
                                    DatumEnds.End0,
                                    view);

                                hiddenBubbleCount++;
                                viewProcessed = true;
                            }
                        }
                        catch (ArgumentException)
                        {
                            // Bubble không tồn tại / Level không thể
                            // thao tác ở đầu này trong View.
                        }
                        catch (InvalidOperationException)
                        {
                            // Level/View không hỗ trợ Bubble operation.
                        }

                        // =================================================
                        // End 1
                        // =================================================

                        try
                        {
                            if (level.IsBubbleVisibleInView(
                                    DatumEnds.End1,
                                    view))
                            {
                                level.HideBubbleInView(
                                    DatumEnds.End1,
                                    view);

                                hiddenBubbleCount++;
                                viewProcessed = true;
                            }
                        }
                        catch (ArgumentException)
                        {
                            // Bubble không tồn tại / Level không thể
                            // thao tác ở đầu này trong View.
                        }
                        catch (InvalidOperationException)
                        {
                            // Level/View không hỗ trợ Bubble operation.
                        }
                    }

                    if (viewProcessed)
                    {
                        processedViewCount++;
                    }
                }

                trans.Commit();
            }

            // =====================================================
            // 5. Thông báo kết quả
            // =====================================================

            if (hiddenBubbleCount == 0)
            {
                TaskDialog.Show(
                    "Hide Level Heads",
                    $"Đã kiểm tra {targetViews.Count} View.\n\n" +
                    "Không có Level Head nào đang hiển thị để ẩn.");
            }
            else
            {
                TaskDialog.Show(
                    "Hide Level Heads",
                    $"Đã ẩn Level Head thành công.\n\n" +
                    $"Views xử lý: {processedViewCount}/{targetViews.Count}\n" +
                    $"Level Heads đã ẩn: {hiddenBubbleCount}");
            }

            return Result.Succeeded;
        }
    }
}

