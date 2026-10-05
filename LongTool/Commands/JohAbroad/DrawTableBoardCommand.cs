using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.Services.DoorBoard;
using LongTool.Core.Selection;
using LongTool.Models;
using LongTool.Services.DoorBoard;
using System;
using System.Collections.Generic;
using System.Linq;

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
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // ==================================================
            // 1. Collect all Door instances
            // ==================================================

            List<FamilyInstance> doors =
                new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .WhereElementIsNotElementType()
                    .OfType<FamilyInstance>()
                    .ToList();

            if (doors.Count == 0)
            {
                TaskDialog.Show(
                    "LongTool - Door Board",
                    "Trong Project không tìm thấy Door nào.");
                return Result.Cancelled;
            }

            // ==================================================
            // 2. Read Door Type Parameters
            // ==================================================

            List<DoorScheduleItem> doorItems =
                doors
                    .Where(d => d.Symbol != null)
                    .Select(BuildDoorScheduleItem)
                    .ToList();

            if (doorItems.Count == 0)
            {
                TaskDialog.Show(
                    "LongTool - Door Board",
                    "Không đọc được thông tin Door Type.");
                return Result.Cancelled;
            }

            // ==================================================
            // 3. Group by 記号
            // ==================================================

            List<DoorScheduleItem> groupedDoors =
                doorItems
                    .GroupBy(x => x.Symbol)
                    .Select(g => MergeGroup(g))
                    .Where(x => !string.IsNullOrWhiteSpace(x.Symbol))
                    .OrderBy(x => x.Symbol)
                    .ToList();

            if (groupedDoors.Count == 0)
            {
                TaskDialog.Show(
                    "LongTool - Door Board",
                    "Không tìm thấy Door nào có J_Name / J_Number.\n\n" +
                    "Hãy kiểm tra các parameter Type.");
                return Result.Cancelled;
            }

            // ==================================================
            // 4. Confirm Door Data
            // ==================================================

            string summary =
                $"Tìm thấy {doors.Count} cửa.\n\n" +
                $"Có {groupedDoors.Count} loại cửa:\n\n";

            foreach (DoorScheduleItem door in groupedDoors)
                summary += $"{door.Symbol}    × {door.Quantity}\n";

            summary += "\nBạn có muốn tiếp tục không?";

            TaskDialogResult confirmResult =
                TaskDialog.Show(
                    "LongTool - Door Board",
                    summary,
                    TaskDialogCommonButtons.Yes |
                    TaskDialogCommonButtons.No);

            if (confirmResult != TaskDialogResult.Yes)
                return Result.Cancelled;

            // ==================================================
            // 5. Check Template Legend
            // ==================================================

            View templateView = doc.ActiveView;

            if (templateView.ViewType != ViewType.Legend)
            {
                TaskDialog.Show(
                    "LongTool - Door Board",
                    "Vui lòng chạy lệnh trong Legend View.\n\n" +
                    "Legend hiện tại sẽ được sử dụng làm Template.");
                return Result.Cancelled;
            }

            TaskDialogResult templateResult =
                TaskDialog.Show(
                    "LongTool - Door Board",
                    $"Legend Template:\n\n" +
                    $"{templateView.Name}\n\n" +
                    "Sử dụng Legend này làm mẫu?",
                    TaskDialogCommonButtons.Yes |
                    TaskDialogCommonButtons.No);

            if (templateResult != TaskDialogResult.Yes)
                return Result.Cancelled;

            // ==================================================
            // 6. Pick Origin
            // ==================================================

            PickPointResult? pick =
                PointPicker.PickPoint(
                    uidoc,
                    "Chọn điểm chuẩn làm Origin của Door Legend");

            if (pick == null)
                return Result.Cancelled;

            XYZ origin = pick.Point;

            // ==================================================
            // 7. Check existing Legend names
            // ==================================================

            List<string> existingLegendNames =
                new FilteredElementCollector(doc)
                    .OfClass(typeof(View))
                    .Cast<View>()
                    .Where(v => v.ViewType == ViewType.Legend)
                    .Select(v => v.Name)
                    .ToList();

            foreach (DoorScheduleItem door in groupedDoors)
            {
                if (existingLegendNames.Any(name =>
                        name.Equals(
                            door.Symbol,
                            StringComparison.OrdinalIgnoreCase)))
                {
                    TaskDialog.Show(
                        "LongTool - Door Board",
                        $"Legend \"{door.Symbol}\" đã tồn tại.\n\n" +
                        "Tool sẽ dừng để tránh ghi đè.");
                    return Result.Cancelled;
                }
            }

            // ==================================================
            // 8. Find Text Type
            // ==================================================

            DoorBoardDrawingService drawingService =
                new DoorBoardDrawingService(doc);

            TextNoteType? textType =
                drawingService.FindFirstTextNoteType();

            if (textType == null)
            {
                TaskDialog.Show(
                    "LongTool - Door Board",
                    "Không tìm thấy TextNoteType để render bảng.");
                return Result.Cancelled;
            }

            // ==================================================
            // 9. Duplicate Legends + Create Tables
            // ==================================================

            List<View> createdViews = new List<View>();

            using (Transaction transaction =
                new Transaction(doc, "Create Door Legends"))
            {
                transaction.Start();

                foreach (DoorScheduleItem door in groupedDoors)
                {
                    View newView =
                        drawingService.DuplicateLegendAndDraw(
                            templateView,
                            door,
                            origin,
                            textType);

                    createdViews.Add(newView);
                }

                transaction.Commit();
            }

            // ==================================================
            // 10. Result
            // ==================================================

            string createdSummary =
                $"Đã tạo {createdViews.Count} Legend:\n\n";

            foreach (View view in createdViews)
                createdSummary += $"• {view.Name}\n";

            createdSummary +=
                "\n" +
                $"Template: {templateView.Name}\n\n" +
                "Đã tạo bảng Door Board tương ứng " +
                "cho từng Legend.";

            TaskDialog.Show(
                "LongTool - Door Board",
                createdSummary);

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

    // ==================================================
    // BUILD DoorScheduleItem từ FamilyInstance
    // ==================================================

    private static DoorScheduleItem BuildDoorScheduleItem(
        FamilyInstance door)
    {
        FamilySymbol symbol = door.Symbol;

        string jName = GetTextParameter(symbol, "J_Name");
        string jNumber = GetTextParameter(symbol, "J_Number");

        string symbolName =
            !string.IsNullOrWhiteSpace(jName) &&
            !string.IsNullOrWhiteSpace(jNumber)
                ? $"{jName}-{jNumber}"
                : !string.IsNullOrWhiteSpace(jName)
                    ? jName
                    : jNumber;

        return new DoorScheduleItem
        {
            Symbol = symbolName,
            Quantity = 1,
            Type = GetTextParameter(symbol, "J_Type"),
            Location = GetTextParameter(symbol, "J_Location"),
            Glass = GetTextParameter(symbol, "J_Glass"),
            Finish = GetTextParameter(symbol, "J_Finish"),
            Hardware = GetTextParameter(symbol, "J_Hardware"),
            Remarks = GetTextParameter(symbol, "J_Remarks"),
            DoorThickness = GetTextParameter(symbol, "J_DoorThickness"),
            FrameThickness = GetTextParameter(symbol, "J_FrameThickness"),
            ShapeWidth = GetLengthParameterInMillimeters(symbol, "Width"),
            ShapeHeight = GetLengthParameterInMillimeters(symbol, "Height")
        };
    }

    private static DoorScheduleItem MergeGroup(
        IGrouping<string, DoorScheduleItem> group)
    {
        DoorScheduleItem first = group.First();

        return new DoorScheduleItem
        {
            Symbol = group.Key,
            Quantity = group.Count(),
            Type = first.Type,
            Location = first.Location,
            Glass = first.Glass,
            Finish = first.Finish,
            Hardware = first.Hardware,
            Remarks = first.Remarks,
            DoorThickness = first.DoorThickness,
            FrameThickness = first.FrameThickness,
            ShapeWidth = first.ShapeWidth,
            ShapeHeight = first.ShapeHeight
        };
    }

    // ==================================================
    // Parameter helpers (giữ nguyên từ file gốc)
    // ==================================================

    private static string GetTextParameter(
        Element element,
        string parameterName)
    {
        Parameter? parameter = element.LookupParameter(parameterName);

        return parameter?.AsString() ?? string.Empty;
    }

    private static double GetLengthParameterInMillimeters(
        Element element,
        string parameterName)
    {
        Parameter? parameter = element.LookupParameter(parameterName);

        if (parameter == null)
            return 0;

        if (parameter.StorageType != StorageType.Double)
            return 0;

        double valueInFeet = parameter.AsDouble();

        return UnitUtils.ConvertFromInternalUnits(
            valueInFeet,
            UnitTypeId.Millimeters);
    }
}