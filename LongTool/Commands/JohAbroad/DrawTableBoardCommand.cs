using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.Core.Selection;
using LongTool.Models;
using LongTool.Tables.Builders;
using LongTool.Tables.Layout;
using LongTool.Tables.Renderers;
using LongTool.Tables.Rendering;
using LongTool.Tables.Revit;
using LongTool.Tables.Services;
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
            UIApplication uiapp =
                commandData.Application;

            UIDocument uidoc =
                uiapp.ActiveUIDocument;

            Document doc =
                uidoc.Document;

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
                new List<DoorScheduleItem>();

            foreach (FamilyInstance door in doors)
            {
                FamilySymbol? symbol =
                    door.Symbol;

                if (symbol == null)
                {
                    continue;
                }

                string jName =
                    GetTextParameter(
                        symbol,
                        "J_Name");

                string jNumber =
                    GetTextParameter(
                        symbol,
                        "J_Number");

                string symbolName =
                    string.Empty;

                if (!string.IsNullOrWhiteSpace(jName) &&
                    !string.IsNullOrWhiteSpace(jNumber))
                {
                    symbolName =
                        $"{jName}-{jNumber}";
                }
                else if (!string.IsNullOrWhiteSpace(jName))
                {
                    symbolName =
                        jName;
                }
                else if (!string.IsNullOrWhiteSpace(jNumber))
                {
                    symbolName =
                        jNumber;
                }

                string jType =
                    GetTextParameter(
                        symbol,
                        "J_Type");

                string jLocation =
                    GetTextParameter(
                        symbol,
                        "J_Location");

                string jGlass =
                    GetTextParameter(
                        symbol,
                        "J_Glass");

                string jFinish =
                    GetTextParameter(
                        symbol,
                        "J_Finish");

                string jHardware =
                    GetTextParameter(
                        symbol,
                        "J_Hardware");

                string jRemarks =
                    GetTextParameter(
                        symbol,
                        "J_Remarks");

                string jDoorThickness =
                    GetTextParameter(
                        symbol,
                        "J_DoorThickness");

                string jFrameThickness =
                    GetTextParameter(
                        symbol,
                        "J_FrameThickness");

                double width =
                    GetLengthParameterInMillimeters(
                        symbol,
                        "Width");

                double height =
                    GetLengthParameterInMillimeters(
                        symbol,
                        "Height");

                DoorScheduleItem item =
                    new DoorScheduleItem
                    {
                        Symbol = symbolName,

                        Quantity = 1,

                        Type = jType,

                        Location = jLocation,

                        Glass = jGlass,

                        Finish = jFinish,

                        Hardware = jHardware,

                        Remarks = jRemarks,

                        DoorThickness =
                            jDoorThickness,

                        FrameThickness =
                            jFrameThickness,

                        ShapeWidth =
                            width,

                        ShapeHeight =
                            height
                    };

                doorItems.Add(item);
            }

            // ==================================================
            // 3. Check result
            // ==================================================

            if (doorItems.Count == 0)
            {
                TaskDialog.Show(
                    "LongTool - Door Board",
                    "Không đọc được thông tin Door Type.");

                return Result.Cancelled;
            }

            // ==================================================
            // 4. Group by 記号
            // ==================================================

            List<DoorScheduleItem> groupedDoors =
                doorItems
                    .GroupBy(x => x.Symbol)
                    .Select(group =>
                        new DoorScheduleItem
                        {
                            Symbol =
                                group.Key,

                            Quantity =
                                group.Count(),

                            Type =
                                group.First().Type,

                            Location =
                                group.First().Location,

                            Glass =
                                group.First().Glass,

                            Finish =
                                group.First().Finish,

                            Hardware =
                                group.First().Hardware,

                            Remarks =
                                group.First().Remarks,

                            DoorThickness =
                                group.First().DoorThickness,

                            FrameThickness =
                                group.First().FrameThickness,

                            ShapeWidth =
                                group.First().ShapeWidth,

                            ShapeHeight =
                                group.First().ShapeHeight
                        })
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Symbol))
                    .OrderBy(x => x.Symbol)
                    .ToList();

            // ==================================================
            // 5. Check Symbol
            // ==================================================

            if (groupedDoors.Count == 0)
            {
                TaskDialog.Show(
                    "LongTool - Door Board",
                    "Không tìm thấy Door nào có J_Name / J_Number.\n\n" +
                    "Hãy kiểm tra các parameter Type.");

                return Result.Cancelled;
            }

            // ==================================================
            // 6. Confirm Door Data
            // ==================================================

            string summary =
                $"Tìm thấy {doors.Count} cửa.\n\n" +
                $"Có {groupedDoors.Count} loại cửa:\n\n";

            foreach (DoorScheduleItem door in groupedDoors)
            {
                summary +=
                    $"{door.Symbol}    × {door.Quantity}\n";
            }

            summary +=
                "\nBạn có muốn tiếp tục không?";

            TaskDialogResult confirmResult =
                TaskDialog.Show(
                    "LongTool - Door Board",
                    summary,
                    TaskDialogCommonButtons.Yes |
                    TaskDialogCommonButtons.No);

            if (confirmResult !=
                TaskDialogResult.Yes)
            {
                return Result.Cancelled;
            }

            // ==================================================
            // 7. Template Legend
            // ==================================================

            View templateView =
                doc.ActiveView;

            if (templateView.ViewType !=
                ViewType.Legend)
            {
                TaskDialog.Show(
                    "LongTool - Door Board",
                    "Vui lòng chạy lệnh trong Legend View.\n\n" +
                    "Legend hiện tại sẽ được sử dụng làm Template.");

                return Result.Cancelled;
            }

            // ==================================================
            // 8. Confirm Template
            // ==================================================

            TaskDialogResult templateResult =
                TaskDialog.Show(
                    "LongTool - Door Board",
                    $"Legend Template:\n\n" +
                    $"{templateView.Name}\n\n" +
                    "Sử dụng Legend này làm mẫu?",
                    TaskDialogCommonButtons.Yes |
                    TaskDialogCommonButtons.No);

            if (templateResult !=
                TaskDialogResult.Yes)
            {
                return Result.Cancelled;
            }

            // ==================================================
            // 9. Pick Origin
            // ==================================================

            PickPointResult? pick =
                PointPicker.PickPoint(
                    uidoc,
                    "Chọn điểm chuẩn làm Origin của Door Legend");

            if (pick == null)
            {
                return Result.Cancelled;
            }

            XYZ origin =
                pick.Point;

            // ==================================================
            // 10. Check existing Legend names
            // ==================================================

            List<string> existingLegendNames =
                new FilteredElementCollector(doc)
                    .OfClass(typeof(View))
                    .Cast<View>()
                    .Where(v =>
                        v.ViewType ==
                        ViewType.Legend)
                    .Select(v => v.Name)
                    .ToList();

            List<string> namesToCreate =
                new List<string>();

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

                if (namesToCreate.Any(name =>
                    name.Equals(
                        door.Symbol,
                        StringComparison.OrdinalIgnoreCase)))
                {
                    TaskDialog.Show(
                        "LongTool - Door Board",
                        $"Có tên Legend bị trùng: {door.Symbol}");

                    return Result.Cancelled;
                }

                namesToCreate.Add(
                    door.Symbol);
            }

            // ==================================================
            // 11. Find Text Type
            // ==================================================

            TextNoteType? textType =
                new FilteredElementCollector(doc)
                    .OfClass(typeof(TextNoteType))
                    .Cast<TextNoteType>()
                    .FirstOrDefault();

            if (textType == null)
            {
                TaskDialog.Show(
                    "LongTool - Door Board",
                    "Không tìm thấy TextNoteType để render bảng.");

                return Result.Cancelled;
            }

            // ==================================================
            // 12. Duplicate Legends + Create Tables
            // ==================================================

            List<View> createdViews =
                new List<View>();

            using (Transaction transaction =
                new Transaction(
                    doc,
                    "Create Door Legends"))
            {
                transaction.Start();

                foreach (DoorScheduleItem door in groupedDoors)
                {
                    // --------------------------------------------------
                    // Duplicate Template Legend
                    // --------------------------------------------------

                    ElementId duplicatedViewId =
                        templateView.Duplicate(
                            ViewDuplicateOption.Duplicate);

                    View? newView =
                        doc.GetElement(
                            duplicatedViewId)
                        as View;

                    if (newView == null)
                    {
                        throw new InvalidOperationException(
                            $"Không thể duplicate Legend cho {door.Symbol}.");
                    }

                    newView.Name =
                        door.Symbol;

                    // --------------------------------------------------
                    // Build Door Table
                    // --------------------------------------------------

                    LongTool.Tables.Models.Table table =
                        DoorBoardBuilder.CreateTable(
                            door);

                    // --------------------------------------------------
                    // Auto Fit
                    // --------------------------------------------------

                    TableTextMeasureService textMeasure =
                        new TableTextMeasureService(
                            doc,
                            newView,
                            textType.Id);

                    TableAutoFitService autoFit =
                        new TableAutoFitService(
                            textMeasure);

                    TableLayout tempLayout =
                        new TableLayout(
                            table);

                    tempLayout.Build();

                    autoFit.AutoFit(
                        tempLayout);

                    // --------------------------------------------------
                    // Build final Layout
                    // --------------------------------------------------

                    TableLayout layout =
                        new TableLayout(
                            table);

                    layout.Build();

                    // --------------------------------------------------
                    // Render
                    // --------------------------------------------------

                    RevitRenderContext context =
                        new RevitRenderContext(
                            doc,
                            newView,
                            origin);

                    BorderRenderer borderRenderer =
                        new BorderRenderer(
                            context);

                    borderRenderer.Render(
                        layout);

                    TableTextRenderer textRenderer =
                        new TableTextRenderer(
                            doc,
                            newView,
                            context);

                    textRenderer.Render(
                        layout);

                    createdViews.Add(
                        newView);
                }

                transaction.Commit();
            }

            // ==================================================
            // 13. Result
            // ==================================================

            string createdSummary =
                $"Đã tạo {createdViews.Count} Legend:\n\n";

            foreach (View view in createdViews)
            {
                createdSummary +=
                    $"• {view.Name}\n";
            }

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
            message =
                ex.ToString();

            TaskDialog.Show(
                "Door Board Error",
                ex.ToString());

            return Result.Failed;
        }
    }

    // ==================================================
    // Get Text Parameter
    // From Family Type
    // ==================================================

    private static string GetTextParameter(
        Element element,
        string parameterName)
    {
        Parameter? parameter =
            element.LookupParameter(
                parameterName);

        if (parameter == null)
        {
            return string.Empty;
        }

        return parameter.AsString()
            ?? string.Empty;
    }

    // ==================================================
    // Get Length Parameter → mm
    // From Family Type
    // ==================================================

    private static double
        GetLengthParameterInMillimeters(
            Element element,
            string parameterName)
    {
        Parameter? parameter =
            element.LookupParameter(
                parameterName);

        if (parameter == null)
        {
            return 0;
        }

        if (parameter.StorageType !=
            StorageType.Double)
        {
            return 0;
        }

        double valueInFeet =
            parameter.AsDouble();

        return UnitUtils.ConvertFromInternalUnits(
            valueInFeet,
            UnitTypeId.Millimeters);
    }
}
