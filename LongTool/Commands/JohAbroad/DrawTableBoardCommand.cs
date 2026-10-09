using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;
using LongTool.Domain.Services;
using LongTool.RevitAdapters.Pickers;
using LongTool.RevitAdapters.Repositories;
using LongTool.RevitAdapters.Ui;
using LongTool.RevitAdapters.Writers;

namespace LongTool.Commands.JohAbroad;

[Transaction(TransactionMode.Manual)]
public class DrawTableBoardCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        try
        {
            var uiApp = commandData.Application;
            var uidoc = uiApp.ActiveUIDocument;
            var doc = uidoc.Document;

            // 1. Thu thập dữ liệu (Revit)
            var doorRepo = new RevitDoorRepository(doc);
            var items = doorRepo.GetAll();

            if (items.Count == 0)
            {
                RevitTaskDialog.ShowInfo("Door Board", "Không tìm thấy Door nào.");
                return Result.Cancelled;
            }

            // 2. Gom nhóm (Domain thuần)
            var groupingService = new DoorScheduleGroupingService();
            var grouped = groupingService.GroupBySymbol(items);

            if (grouped.Count == 0)
            {
                RevitTaskDialog.ShowInfo(
                    "Door Board",
                    "Không tìm thấy Door nào có J_Name / J_Number.");
                return Result.Cancelled;
            }

            // 3. Confirm (Domain build string + Revit show dialog)
            var summary = DoorBoardSummaryBuilder.BuildGroupingSummary(
                items.Count, grouped);

            if (!RevitTaskDialog.Confirm("Door Board", summary))
                return Result.Cancelled;

            // 4. Kiểm tra Legend template (Revit)
            var legendRepo = new RevitLegendRepository(doc);
            var templateView = legendRepo.GetActiveLegendOrNull();

            if (templateView == null)
            {
                RevitTaskDialog.ShowInfo(
                    "Door Board",
                    "Vui lòng chạy lệnh trong Legend View.");
                return Result.Cancelled;
            }

            // 5. Pick origin (Revit)
            var picker = new RevitPointPicker(uidoc);
            var origin = picker.Pick("Chọn điểm chuẩn làm Origin của Door Legend");
            if (origin == null) return Result.Cancelled;

            // 6. Check trùng tên Legend (Revit)
            var conflict = grouped.FirstOrDefault(d => legendRepo.Exists(d.Symbol));
            if (conflict != null)
            {
                RevitTaskDialog.ShowInfo(
                    "Door Board",
                    $"Legend \"{conflict.Symbol}\" đã tồn tại.");
                return Result.Cancelled;
            }

            // 7. Tạo Legend (Revit + Transaction)
            var writer = new RevitLegendWriter(doc);
            var createdNames = writer.CreateDoorLegends(templateView, grouped, origin);

            // 8. Kết quả
            var resultMsg = DoorBoardSummaryBuilder.BuildCreatedSummary(
                createdNames, templateView.Name);

            RevitTaskDialog.ShowInfo("Door Board", resultMsg);

            return Result.Succeeded;
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
        {
            return Result.Cancelled;
        }
        catch (Exception ex)
        {
            message = ex.ToString();
            TaskDialog.Show("Door Board Error", ex.ToString());
            return Result.Failed;
        }
    }
}