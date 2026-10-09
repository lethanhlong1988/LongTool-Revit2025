using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.Core.Selection;
using LongTool.Core.Storage;
using LongTool.Models;
using LongTool.Services.DoorBoard;
using LongTool.UI.Common.FormDialog;
using LongTool.UI.Common.SelectionDialog;
using LongTool.UI.Common.SelectionDialog.Providers;
using System;
using System.Linq;

namespace LongTool.Commands.Test;

[Transaction(TransactionMode.Manual)]
public class TestDoorBoardCommand : IExternalCommand
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

            if (!HasDoor(doc))
            {
                TaskDialog.Show(
                    "Door Board",
                    "Trong model không có Door nào.");

                return Result.Cancelled;
            }

            if (!HasLegend(doc))
            {
                TaskDialog.Show(
                    "Door Board",
                    "Trong model chưa có Legend nào.\n\n" +
                    "Hãy tạo ít nhất một Legend để làm template.");

                return Result.Cancelled;
            }

            // ==================================================
            // 1. Chọn Door symbol
            // ==================================================

            FamilySymbol? doorSymbol =
                SelectDoorSymbol(doc);

            if (doorSymbol == null)
                return Result.Cancelled;

            // ==================================================
            // 2. Chọn Legend Component (mặt đứng cửa)
            // ==================================================

            FamilySymbol? legendSymbol =
                SelectLegendSymbol(doc);

            if (legendSymbol == null)
                return Result.Cancelled;

            ElementId legendSymbolId = legendSymbol.Id;

            // ==================================================
            // 3. Build DoorScheduleItem
            // ==================================================

            DoorScheduleItem item =
                BuildDoorScheduleItem(doorSymbol);

            string targetLegendName =
                GetTargetLegendName(item);

            DoorBoardDrawingService service =
                new DoorBoardDrawingService(doc);

            TextNoteType? textType =
                service.FindFirstTextNoteType();

            if (textType == null)
            {
                TaskDialog.Show(
                    "Door Board",
                    "Không tìm thấy TextNoteType trong document.");

                return Result.Cancelled;
            }

            View? existingLegend =
                service.FindLegendByName(targetLegendName);

            if (existingLegend != null)
            {
                return UpdateExistingLegend(
                    uidoc,
                    doc,
                    service,
                    existingLegend,
                    item,
                    textType,
                    targetLegendName,
                    legendSymbolId);
            }

            View? legendTemplate =
                SelectLegendTemplate(doc);

            if (legendTemplate == null)
                return Result.Cancelled;

            return CreateNewLegend(
                uidoc,
                doc,
                service,
                legendTemplate,
                item,
                textType,
                targetLegendName,
                doorSymbol,
                legendSymbolId);
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
        {
            return Result.Cancelled;
        }
        catch (Exception ex)
        {
            message = ex.ToString();

            TaskDialog.Show(
                "Door Board Error",
                ex.ToString());

            return Result.Failed;
        }
    }

    // ==================================================
    // EXISTENCE CHECKS
    // ==================================================

    private static bool HasDoor(Document doc)
    {
        return new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_Doors)
            .WhereElementIsNotElementType()
            .Any();
    }

    private static bool HasLegend(Document doc)
    {
        return new FilteredElementCollector(doc)
            .OfClass(typeof(View))
            .Cast<View>()
            .Any(v =>
                v.ViewType == ViewType.Legend &&
                !v.IsTemplate);
    }

    // ==================================================
    // SELECTIONS
    // ==================================================

    private static FamilySymbol? SelectDoorSymbol(
        Document doc)
    {
        var fields =
            new (string Key, ISelectionProvider Provider)[]
            {
                (
                    "Door",
                    new DoorSelectionProvider(doc)
                )
            };

        var form = new FormDialogView(
            "Door Board - Chọn Door",
            fields);

        if (form.ShowDialog() != true)
            return null;

        return form.Results["Door"].Value
            as FamilySymbol;
    }

    private static FamilySymbol? SelectLegendSymbol(
        Document doc)
    {
        var fields =
            new (string Key, ISelectionProvider Provider)[]
            {
                (
                    "LegendSymbol",
                    new LegendComponentSelectionProvider(doc)
                )
            };

        var form = new FormDialogView(
            "Door Board - Chọn Legend Component (mặt đứng cửa)",
            fields);

        if (form.ShowDialog() != true)
            return null;

        return form.Results["LegendSymbol"].Value
            as FamilySymbol;
    }

    private static View? SelectLegendTemplate(
        Document doc)
    {
        var fields =
            new (string Key, ISelectionProvider Provider)[]
            {
                (
                    "Legend",
                    new LegendSelectionProvider(doc)
                )
            };

        var form = new FormDialogView(
            "Door Board - Chọn Legend Template",
            fields);

        if (form.ShowDialog() != true)
            return null;

        return form.Results["Legend"].Value
            as View;
    }

    // ==================================================
    // UPDATE EXISTING LEGEND
    // ==================================================

    private static Result UpdateExistingLegend(
        UIDocument uidoc,
        Document doc,
        DoorBoardDrawingService service,
        View existingLegend,
        DoorScheduleItem item,
        TextNoteType textType,
        string targetLegendName,
        ElementId legendSymbolId)
    {
        // ------------------------------------------------------------
        // 1. Đọc metadata của Legend hiện tại
        // ------------------------------------------------------------

        DoorBoardMetadata? meta =
            service.GetMetadata(existingLegend);

        if (meta == null)
        {
            TaskDialog.Show(
                "Door Board",
                $"Legend \"{targetLegendName}\" đã tồn tại " +
                "nhưng không có metadata.\n\n" +
                "Tool không thể xác định vị trí ban đầu " +
                "của bảng Door Board.\n\n" +
                "Vui lòng xóa Legend này và tạo lại bằng Tool.");

            return Result.Cancelled;
        }

        XYZ origin = meta.Origin;

        // ------------------------------------------------------------
        // 2. Tạo tên tạm cho Legend mới
        // ------------------------------------------------------------

        string temporaryName =
            GetTemporaryLegendName(
                service,
                targetLegendName);

        View newLegend;

        // ------------------------------------------------------------
        // 3. Toàn bộ Update nằm trong một Transaction
        // ------------------------------------------------------------

        using (Transaction trans =
            new Transaction(doc, "Update Door Board"))
        {
            trans.Start();

            // --------------------------------------------------------
            // 3.1 Duplicate Legend cũ
            // --------------------------------------------------------

            newLegend =
                service.DuplicateLegend(
                    existingLegend,
                    temporaryName);

            // --------------------------------------------------------
            // 3.2 Vẽ Door Board mới tại đúng Origin cũ
            // --------------------------------------------------------

            service.Draw(
                newLegend,
                item,
                origin,
                textType,
                legendSymbolId);

            // --------------------------------------------------------
            // 3.3 Lưu metadata cho Legend mới
            // --------------------------------------------------------

            service.SaveMetadata(
                newLegend,
                origin,
                item.Symbol,
                meta.CreatedAt);

            // --------------------------------------------------------
            // 3.4 Xóa Legend cũ
            // --------------------------------------------------------

            doc.Delete(existingLegend.Id);

            // --------------------------------------------------------
            // 3.5 Đổi tên Legend mới thành tên chính thức
            // --------------------------------------------------------

            newLegend.Name = targetLegendName;

            // --------------------------------------------------------
            // 3.6 Commit
            // --------------------------------------------------------

            trans.Commit();
        }

        // ------------------------------------------------------------
        // 4. Chuyển sang Legend mới
        // ------------------------------------------------------------

        uidoc.RequestViewChange(newLegend);

        TaskDialog.Show(
            "Door Board",
            $"Đã cập nhật bảng Door Board \"{targetLegendName}\".\n\n" +
            "Legend cũ đã được thay thế bằng Legend mới.\n" +
            "Vị trí bảng được giữ nguyên theo metadata.");

        return Result.Succeeded;
    }

    private static string GetTemporaryLegendName(
        DoorBoardDrawingService service,
        string targetLegendName)
    {
        string baseName =
            $"{targetLegendName}_copy";

        string temporaryName = baseName;

        int index = 1;

        while (service.FindLegendByName(temporaryName) != null)
        {
            temporaryName =
                $"{baseName}_{index:00}";

            index++;
        }

        return temporaryName;
    }

    // ==================================================
    // CREATE NEW LEGEND
    // ==================================================

    private static Result CreateNewLegend(
        UIDocument uidoc,
        Document doc,
        DoorBoardDrawingService service,
        View legendTemplate,
        DoorScheduleItem item,
        TextNoteType textType,
        string targetLegendName,
        FamilySymbol doorSymbol,
        ElementId legendSymbolId)
    {
        if (doc.ActiveView.Id != legendTemplate.Id)
        {
            uidoc.RequestViewChange(legendTemplate);
        }

        PickPointResult? pick =
            PointPicker.PickPoint(
                uidoc,
                $"Chọn điểm đặt bảng Door Board trong Legend \"{legendTemplate.Name}\"");

        if (pick == null)
            return Result.Cancelled;

        XYZ newOrigin = pick.Point;

        View createdView;

        using (Transaction trans =
            new Transaction(doc, "Create Door Board"))
        {
            trans.Start();

            createdView =
                service.DuplicateLegend(
                    legendTemplate,
                    targetLegendName);

            service.Draw(
                createdView,
                item,
                newOrigin,
                textType,
                legendSymbolId);

            service.SaveMetadata(
                createdView,
                newOrigin,
                item.Symbol);

            trans.Commit();
        }

        uidoc.RequestViewChange(createdView);

        TaskDialog.Show(
            "Door Board",
            $"Đã tạo mới bảng Door Board \"{targetLegendName}\".\n\n" +
            $"Type: {doorSymbol.Name}\n" +
            "Vị trí đã được lưu để sử dụng cho các lần Update sau.");

        return Result.Succeeded;
    }

    // ==================================================
    // BUILD ITEM
    // ==================================================

    private static string GetTargetLegendName(
        DoorScheduleItem item)
    {
        return string.IsNullOrWhiteSpace(item.Symbol)
            ? $"DoorBoard_Test_{DateTime.Now:HHmmss}"
            : item.Symbol;
    }

    private static DoorScheduleItem BuildDoorScheduleItem(
        FamilySymbol symbol)
    {
        string jName =
            GetTextParameter(symbol, "J_Name");

        string jNumber =
            GetTextParameter(symbol, "J_Number");

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

            Type =
                GetTextParameter(symbol, "J_Type"),

            Location =
                GetTextParameter(symbol, "J_Location"),

            Glass =
                GetTextParameter(symbol, "J_Glass"),

            Finish =
                GetTextParameter(symbol, "J_Finish"),

            Hardware =
                GetTextParameter(symbol, "J_Hardware"),

            Remarks =
                GetTextParameter(symbol, "J_Remarks"),

            DoorThickness =
                GetTextParameter(
                    symbol,
                    "J_DoorThickness"),

            FrameThickness =
                GetTextParameter(
                    symbol,
                    "J_FrameThickness"),

            ShapeWidth =
                GetLengthParameterInMillimeters(
                    symbol,
                    "Width"),

            ShapeHeight =
                GetLengthParameterInMillimeters(
                    symbol,
                    "Height")
        };
    }

    private static string GetTextParameter(
        Element? element,
        string parameterName)
    {
        if (element == null)
            return string.Empty;

        Parameter? parameter =
            element.LookupParameter(parameterName);

        return parameter?.AsString()
            ?? string.Empty;
    }

    private static double GetLengthParameterInMillimeters(
        Element? element,
        string parameterName)
    {
        if (element == null)
            return 0;

        Parameter? parameter =
            element.LookupParameter(parameterName);

        if (parameter == null ||
            parameter.StorageType != StorageType.Double)
        {
            return 0;
        }

        return UnitUtils.ConvertFromInternalUnits(
            parameter.AsDouble(),
            UnitTypeId.Millimeters);
    }
}