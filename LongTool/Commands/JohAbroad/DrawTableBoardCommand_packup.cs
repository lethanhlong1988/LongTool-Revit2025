//using Autodesk.Revit.Attributes;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
//using LongTool.Core.Selection;
//using LongTool.Tables.Builders;
//using LongTool.Tables.Layout;
//using LongTool.Tables.Renderers;
//using LongTool.Tables.Rendering;
//using LongTool.Tables.Revit;
//using LongTool.Tables.Services;
//using LongTool.UI.Table;
//using System;
//using System.Linq;
//using LongTool.Tables.Models;

//namespace LongTool.Commands.JohAbroad;

//[Transaction(TransactionMode.Manual)]
//public class DrawTableBoardCommand : IExternalCommand
//{
//    public Result Execute(
//        ExternalCommandData commandData,
//        ref string message,
//        ElementSet elements)
//    {
//        try
//        {
//            UIApplication uiapp =
//                commandData.Application;

//            UIDocument uidoc =
//                uiapp.ActiveUIDocument;

//            Document doc =
//                uidoc.Document;

//            // ==================================================
//            // 1. Template Legend View
//            // ==================================================

//            View templateView =
//                doc.ActiveView;

//            if (templateView.ViewType != ViewType.Legend)
//            {
//                TaskDialog.Show(
//                    "LongTool",
//                    "Vui lòng chạy lệnh trong Legend View.\n\n" +
//                    "Legend View hiện tại sẽ được sử dụng làm Template.");

//                return Result.Cancelled;
//            }

//            // ==================================================
//            // 2. Pick Origin trên Template
//            // ==================================================

//            PickPointResult? pick =
//                PointPicker.PickPoint(
//                    uidoc,
//                    "Chọn điểm chuẩn làm Origin của Door Board");

//            if (pick == null)
//            {
//                return Result.Cancelled;
//            }

//            XYZ origin =
//                pick.Point;

//            // ==================================================
//            // 3. Nhập tên Legend View mới
//            // ==================================================

//            TableNameWindow window =
//                new TableNameWindow();

//            bool? result =
//                window.ShowDialog();

//            if (result != true)
//            {
//                return Result.Cancelled;
//            }

//            string newViewName =
//                window.TableName.Trim();

//            if (string.IsNullOrEmpty(newViewName))
//            {
//                TaskDialog.Show(
//                    "LongTool",
//                    "Tên Legend View không được để trống.");

//                return Result.Cancelled;
//            }

//            // ==================================================
//            // 4. Kiểm tra tên Legend View
//            // ==================================================

//            bool viewExists =
//                new FilteredElementCollector(doc)
//                    .OfClass(typeof(View))
//                    .Cast<View>()
//                    .Any(v =>
//                        v.ViewType == ViewType.Legend &&
//                        v.Name.Equals(
//                            newViewName,
//                            StringComparison.OrdinalIgnoreCase));

//            if (viewExists)
//            {
//                TaskDialog.Show(
//                    "LongTool",
//                    $"Legend View \"{newViewName}\" đã tồn tại.");

//                return Result.Cancelled;
//            }

//            // ==================================================
//            // 5. Duplicate Legend Template
//            //
//            // Không tạo Legend bằng CreateLegend vì
//            // Revit API 2025 không có View.CreateLegend().
//            // ==================================================

//            View newView = null;

//            using (Transaction transaction =
//                new Transaction(
//                    doc,
//                    "Create Door Board Legend"))
//            {
//                transaction.Start();

//                ElementId duplicatedViewId =
//                    templateView.Duplicate(
//                        ViewDuplicateOption.Duplicate);

//                newView =
//                    doc.GetElement(
//                        duplicatedViewId)
//                    as View;

//                if (newView == null)
//                {
//                    throw new InvalidOperationException(
//                        "Không thể duplicate Legend Template.");
//                }

//                newView.Name =
//                    newViewName;

//                transaction.Commit();
//            }

//            // ==================================================
//            // 6. Tạo Door Board
//            // ==================================================

//            Table table =
//                DoorBoardBuilder.CreateMergeTestTable();

//            // ==================================================
//            // 7. AutoFit
//            // ==================================================

//            using (Transaction transaction =
//                new Transaction(
//                    doc,
//                    "AutoFit Door Board"))
//            {
//                transaction.Start();

//                TextNoteType textType =
//                    new FilteredElementCollector(doc)
//                        .OfClass(typeof(TextNoteType))
//                        .FirstElement()
//                        as TextNoteType;

//                if (textType == null)
//                {
//                    throw new InvalidOperationException(
//                        "Không tìm thấy TextNoteType.");
//                }

//                TableTextMeasureService textMeasure =
//                    new TableTextMeasureService(
//                        doc,
//                        newView,
//                        textType.Id);

//                TableAutoFitService autoFit =
//                    new TableAutoFitService(
//                        textMeasure);

//                TableLayout tempLayout =
//                    new TableLayout(table);

//                tempLayout.Build();

//                autoFit.AutoFit(
//                    tempLayout);

//                transaction.Commit();
//            }

//            // ==================================================
//            // 8. Build Layout
//            // ==================================================

//            TableLayout layout =
//                new TableLayout(table);

//            layout.Build();

//            // ==================================================
//            // 9. Render Door Board vào Legend View
//            // ==================================================

//            RevitRenderContext context =
//                new RevitRenderContext(
//                    doc,
//                    newView,
//                    origin);

//            using (Transaction transaction =
//                new Transaction(
//                    doc,
//                    "Render Door Board"))
//            {
//                transaction.Start();

//                // ----------------------------------------------
//                // Border
//                // ----------------------------------------------

//                BorderRenderer borderRenderer =
//                    new BorderRenderer(context);

//                borderRenderer.Render(
//                    layout);

//                // ----------------------------------------------
//                // Text
//                // ----------------------------------------------

//                TableTextRenderer textRenderer =
//                    new TableTextRenderer(
//                        doc,
//                        newView,
//                        context);

//                textRenderer.Render(
//                    layout);

//                transaction.Commit();
//            }

//            // ==================================================
//            // 10. Chuyển sang Legend View mới
//            // ==================================================

//            uidoc.RequestViewChange(
//                newView);

//            // ==================================================
//            // 11. Kết quả
//            // ==================================================

//            TaskDialog.Show(
//                "Door Board",
//                $"Đã tạo Door Board thành công.\n\n" +
//                $"Template:\n" +
//                $"{templateView.Name}\n\n" +
//                $"New View:\n" +
//                $"{newView.Name}\n\n" +
//                $"View Type:\n" +
//                $"Legend\n\n" +
//                $"Origin:\n" +
//                $"X = {origin.X:F6} ft\n" +
//                $"Y = {origin.Y:F6} ft\n" +
//                $"Z = {origin.Z:F6} ft\n\n" +
//                $"Cells: {layout.Cells.Count}\n" +
//                $"Lines: {layout.BorderLines.Count}");

//            return Result.Succeeded;
//        }
//        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
//        {
//            return Result.Cancelled;
//        }
//        catch (Exception ex)
//        {
//            message =
//                ex.ToString();

//            TaskDialog.Show(
//                "Door Board Error",
//                ex.ToString());

//            return Result.Failed;
//        }
//    }
//}
