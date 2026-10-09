using Autodesk.Revit.DB;
using LongTool.Core.Storage;
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

namespace LongTool.Services.DoorBoard;

/// <summary>
/// Service chịu trách nhiệm vẽ bảng Door Board
/// cho MỘT loại cửa (một DoorScheduleItem) lên một Legend View.
/// </summary>
public class DoorBoardDrawingService
{
    private readonly Document _doc;

    public DoorBoardDrawingService(Document doc)
    {
        _doc = doc ?? throw new ArgumentNullException(nameof(doc));
    }

    // ==================================================
    // CORE - DRAW
    // ==================================================

    /// <summary>
    /// Vẽ bảng Door Board lên Legend View đã có sẵn.
    /// </summary>
    /// <param name="legendView">View Legend để vẽ bảng.</param>
    /// <param name="door">Thông tin cửa.</param>
    /// <param name="origin">Điểm gốc đặt bảng.</param>
    /// <param name="textType">TextNoteType dùng cho text.</param>
    /// <param name="legendSymbolId">
    /// ElementId của FamilySymbol (Legend Component mặt đứng cửa).
    /// Nếu null → bỏ qua bước đặt legend component.
    /// </param>
    public void Draw(
        View legendView,
        DoorScheduleItem door,
        XYZ origin,
        TextNoteType textType,
        ElementId? legendSymbolId = null)
    {
        if (legendView == null)
            throw new ArgumentNullException(nameof(legendView));
        if (door == null)
            throw new ArgumentNullException(nameof(door));
        if (origin == null)
            throw new ArgumentNullException(nameof(origin));
        if (textType == null)
            throw new ArgumentNullException(nameof(textType));

        // 1. Build Door Table
        Tables.Models.Table table =
            DoorBoardBuilder.CreateTable(door, 30);

        // 2. Auto Fit
        TableTextMeasureService textMeasure =
            new TableTextMeasureService(
                _doc,
                legendView,
                textType.Id);

        TableAutoFitService autoFit =
            new TableAutoFitService(textMeasure);

        TableLayout tempLayout = new TableLayout(table);
        tempLayout.Build();
        autoFit.AutoFit(tempLayout);

        // 3. Build final Layout
        TableLayout layout = new TableLayout(table);
        layout.Build();

        // 4. Render
        RevitRenderContext context =
            new RevitRenderContext(
                _doc,
                legendView,
                origin);

        BorderRenderer borderRenderer = new BorderRenderer(context);
        borderRenderer.Render(layout);

        TableTextRenderer textRenderer =
            new TableTextRenderer(_doc, legendView, context);

        // Truyền legend symbol để TableTextRenderer đặt
        // legend component vào ô Cell_B7_C7_Merged.
        textRenderer.LegendSymbolId = legendSymbolId;

        textRenderer.Render(layout);
    }

    // ==================================================
    // DUPLICATE
    // ==================================================

    /// <summary>
    /// Chỉ duplicate Legend template thành Legend mới.
    /// KHÔNG vẽ gì cả.
    /// </summary>
    public View DuplicateLegend(View templateView, string newName)
    {
        if (templateView == null)
            throw new ArgumentNullException(nameof(templateView));

        ElementId duplicatedViewId =
            templateView.Duplicate(ViewDuplicateOption.Duplicate);

        View? newView =
            _doc.GetElement(duplicatedViewId) as View;

        if (newView == null)
            throw new InvalidOperationException(
                "Không thể duplicate Legend.");

        if (!string.IsNullOrWhiteSpace(newName))
            newView.Name = newName;

        return newView;
    }

    /// <summary>
    /// Duplicate Legend template và vẽ bảng (API cũ, giữ để tương thích).
    /// </summary>
    public View DuplicateLegendAndDraw(
        View templateView,
        DoorScheduleItem door,
        XYZ origin,
        TextNoteType textType,
        ElementId? legendSymbolId = null)
    {
        if (door == null)
            throw new ArgumentNullException(nameof(door));

        View newView = DuplicateLegend(templateView, door.Symbol);

        Draw(newView, door, origin, textType, legendSymbolId);

        return newView;
    }

    // ==================================================
    // CLEAR CONTENT
    // ==================================================

    /// <summary>
    /// Xóa toàn bộ element bên trong Legend (giữ lại Legend view).
    /// Phải gọi trong Transaction.
    /// </summary>
    public void ClearContent(View legend)
    {
        if (legend == null)
            throw new ArgumentNullException(nameof(legend));

        List<ElementId> elementIds =
            new FilteredElementCollector(_doc, legend.Id)
                .WhereElementIsNotElementType()
                .ToElementIds()
                .ToList();

        if (elementIds.Count > 0)
        {
            _doc.Delete(elementIds);
        }
    }

    // ==================================================
    // METADATA
    // ==================================================

    /// <summary>
    /// Đọc metadata (origin, symbol, ...) đã lưu trong Legend.
    /// Trả về null nếu Legend chưa từng được ghi metadata.
    /// </summary>
    public DoorBoardMetadata? GetMetadata(View legend)
    {
        return DoorBoardMetadataSchema.Read(legend);
    }

    /// <summary>
    /// Ghi metadata vào Legend. Phải gọi trong Transaction.
    /// </summary>
    public void SaveMetadata(
        View legend,
        XYZ origin,
        string symbol,
        string? createdAtOverride = null)
    {
        DoorBoardMetadataSchema.Write(
            legend,
            origin,
            symbol,
            createdAtOverride);
    }

    // ==================================================
    // FINDERS
    // ==================================================

    /// <summary>
    /// Tìm TextNoteType đầu tiên trong document.
    /// </summary>
    public TextNoteType? FindFirstTextNoteType()
    {
        return new FilteredElementCollector(_doc)
            .OfClass(typeof(TextNoteType))
            .Cast<TextNoteType>()
            .FirstOrDefault();
    }

    /// <summary>
    /// Tìm Legend View theo tên (không phân biệt hoa thường).
    /// Trả về null nếu không tìm thấy.
    /// </summary>
    public View? FindLegendByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return new FilteredElementCollector(_doc)
            .OfClass(typeof(View))
            .Cast<View>()
            .FirstOrDefault(v =>
                v.ViewType == ViewType.Legend &&
                v.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}