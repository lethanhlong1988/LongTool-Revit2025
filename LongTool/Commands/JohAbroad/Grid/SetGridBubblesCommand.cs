using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LongTool.Commands.JohAbroad.Grid
{
    [Transaction(TransactionMode.Manual)]
    public class SetGridBubblesCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            if (uidoc == null)
            {
                message = "Không có tài liệu Revit đang mở.";
                return Result.Failed;
            }

            Document doc = uidoc.Document;
            View activeView = doc.ActiveView;

            // Chỉ cho phép chạy trên mặt bằng
            if (!IsPlanView(activeView))
            {
                TaskDialog.Show(
                    "Grid Bubble",
                    "Lệnh này chỉ hoạt động trên mặt bằng Floor Plan hoặc Ceiling Plan.");

                return Result.Cancelled;
            }

            // Tạm thời chọn thao tác bằng TaskDialog.
            // Sau khi kiểm tra logic hoạt động ổn định,
            // sẽ chuyển phần này sang ComboBox trên Ribbon.
            TaskDialog dialog = new TaskDialog("Grid Bubble");

            dialog.MainInstruction = "Chọn thao tác với đầu trục 2D:";

            dialog.AddCommandLink(
                TaskDialogCommandLinkId.CommandLink1,
                "Ẩn đầu trục",
                "Ẩn cả hai đầu ký hiệu 2D của tất cả Grid trong mặt bằng hiện tại.");

            dialog.AddCommandLink(
                TaskDialogCommandLinkId.CommandLink2,
                "Hiện đầu trục",
                "Hiện cả hai đầu ký hiệu 2D của tất cả Grid trong mặt bằng hiện tại.");

            dialog.CommonButtons = TaskDialogCommonButtons.Cancel;

            TaskDialogResult result = dialog.Show();

            if (result == TaskDialogResult.CommandLink1)
            {
                return SetGridBubbles(
                    uidoc,
                    activeView,
                    show: false,
                    ref message);
            }

            if (result == TaskDialogResult.CommandLink2)
            {
                return SetGridBubbles(
                    uidoc,
                    activeView,
                    show: true,
                    ref message);
            }

            return Result.Cancelled;
        }

        /// <summary>
        /// Kiểm tra View hiện tại có phải là mặt bằng hay không.
        /// </summary>
        private static bool IsPlanView(View view)
        {
            return view.ViewType == ViewType.FloorPlan ||
                   view.ViewType == ViewType.CeilingPlan;
        }

        /// <summary>
        /// Ẩn hoặc hiện Bubble 2D của tất cả Grid trong View hiện tại.
        /// </summary>
        private static Result SetGridBubbles(
            UIDocument uidoc,
            View view,
            bool show,
            ref string message)
        {
            Document doc = uidoc.Document;

            // Collect tất cả Grid đang hiển thị trong View hiện tại.
            List<Autodesk.Revit.DB.Grid> grids =
                new FilteredElementCollector(doc, view.Id)
                    .OfClass(typeof(Autodesk.Revit.DB.Grid))
                    .Cast<Autodesk.Revit.DB.Grid>()
                    .ToList();

            if (grids.Count == 0)
            {
                TaskDialog.Show(
                    "Grid Bubble",
                    "Không tìm thấy Grid nào trong mặt bằng hiện tại.");

                return Result.Succeeded;
            }

            int gridCount = 0;
            int bubbleCount = 0;

            using (Transaction transaction =
                   new Transaction(doc, show
                       ? "Show Grid Bubbles"
                       : "Hide Grid Bubbles"))
            {
                transaction.Start();

                foreach (Autodesk.Revit.DB.Grid grid in grids)
                {
                    bool gridProcessed = false;

                    // Xử lý đầu End0
                    if (SetBubbleVisibility(grid, view, DatumEnds.End0, show))
                    {
                        bubbleCount++;
                        gridProcessed = true;
                    }

                    // Xử lý đầu End1
                    if (SetBubbleVisibility(grid, view, DatumEnds.End1, show))
                    {
                        bubbleCount++;
                        gridProcessed = true;
                    }

                    if (gridProcessed)
                    {
                        gridCount++;
                    }
                }

                transaction.Commit();
            }

            uidoc.RefreshActiveView();

            string action = show ? "hiện" : "ẩn";

            TaskDialog.Show(
                "Grid Bubble",
                $"Đã {action} đầu trục 2D cho {gridCount} Grid.\n\n" +
                $"Tổng số đầu trục được xử lý: {bubbleCount}.");

            return Result.Succeeded;
        }

        /// <summary>
        /// Thiết lập trạng thái Bubble của một đầu Grid.
        /// Chỉ tác động đến Bubble 2D trong View được chỉ định.
        /// </summary>
        private static bool SetBubbleVisibility(
            Autodesk.Revit.DB.Grid grid,
            View view,
            DatumEnds end,
            bool show)
        {
            try
            {
                if (show)
                {
                    grid.ShowBubbleInView(end, view);
                }
                else
                {
                    grid.HideBubbleInView(end, view);
                }

                return true;
            }
            catch (Exception)
            {
                // Một số Grid có thể không hỗ trợ Bubble
                // ở một đầu cụ thể trong View hiện tại.
                return false;
            }
        }
    }
}