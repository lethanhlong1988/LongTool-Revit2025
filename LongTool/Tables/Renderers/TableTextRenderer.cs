using Autodesk.Revit.DB;
using LongTool.Tables.Diagnostics;
using LongTool.Tables.Geometry;
using LongTool.Tables.Layout;
using LongTool.Tables.Models.Styles;
using LongTool.Tables.Revit;
using LongTool.Tables.Services;
using System;
using System.Diagnostics;

namespace LongTool.Tables.Renderers;

public class TableTextRenderer
{
    private readonly Document _doc;
    private readonly View _view;
    private readonly RevitRenderContext _context;
    private readonly ElementId _textTypeId;
    private readonly DebugMarkerRenderer _debugMarker;
    private const bool DebugMode = false;
    private readonly TableTextMeasureService _measureService;

    //private const double MmToFeet = 1.0 / 304.8;
    private const double TextHeightMm = 2.5;

    // ==================================================
    // Cell đặc biệt: đặt legend component vào tâm ô
    // ==================================================

    private const string LegendCellName = "Cell_B7_C7_Merged";

    /// <summary>
    /// FamilySymbol của legend component (mặt đứng cửa).
    /// Set từ bên ngoài trước khi gọi Render().
    /// </summary>
    public ElementId LegendSymbolId { get; set; }

    public TableTextRenderer(Document doc, View view, RevitRenderContext context)
    {
        _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _context = context ?? throw new ArgumentNullException(nameof(context));

        _textTypeId = FindTextType();
        _measureService =
            new TableTextMeasureService(
                _doc,
                _view,
                _textTypeId);
        _debugMarker = new DebugMarkerRenderer(_doc, _view);

        SetupTextType();
    }

    public void Render(TableLayout layout)
    {
        foreach (var cellLayout in layout.Cells)
        {
            RenderCell(cellLayout);
        }
    }

    private void RenderCell(TableCellLayout cellLayout)
    {
        // ==================================================
        // Cell đặc biệt: đặt legend component thay vì text
        // ==================================================

        if (cellLayout.Cell.Name == LegendCellName)
        {
            PlaceLegendComponent(cellLayout);
            return;
        }

        // ==================================================
        // Cell thường: vẽ text như cũ
        // ==================================================

        string text = cellLayout.Cell.Text;

        if (string.IsNullOrWhiteSpace(text))
            return;

        try
        {
            XYZ position = _context.ToXYZ(cellLayout.TextAnchor);

            TextNote note = TextNote.Create(
                _doc,
                _view.Id,
                position,
                text,
                _textTypeId);

            if (note == null)
                return;

            note.HorizontalAlignment =
                ConvertHorizontalAlignment(
                    cellLayout.Style.HorizontalAlignment);

            note.VerticalAlignment =
                ConvertVerticalAlignment(
                    cellLayout.Style.VerticalAlignment);

            _doc.Regenerate();

            TextMeasureResult result =
                _measureService.Measure(
                    text,
                    position);

            if (DebugMode)
            {
                _debugMarker.DrawCross(position, 0.01);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    // ==================================================
    // PlaceLegendComponent
    // Đặt legend component vào tâm ô cellLayout
    // ==================================================

    private void PlaceLegendComponent(TableCellLayout cellLayout)
    {
        if (LegendSymbolId == null ||
            LegendSymbolId == ElementId.InvalidElementId)
        {
            // Không có symbol -> bỏ qua
            Debug.WriteLine(
                "LegendSymbolId chưa được set, bỏ qua đặt legend.");
            return;
        }

        try
        {
            FamilySymbol symbol =
                _doc.GetElement(LegendSymbolId) as FamilySymbol;

            if (symbol == null)
            {
                Debug.WriteLine(
                    "LegendSymbolId không phải FamilySymbol.");
                return;
            }

            if (!symbol.IsActive)
                symbol.Activate();

            _doc.Regenerate();

            // ----- Lấy tâm ô -----
            XYZ position = _context.ToXYZ(cellLayout.Center);

            // ----- Đặt legend component -----
            FamilyInstance instance =
                _doc.Create.NewFamilyInstance(
                    position,
                    symbol,
                    _view);

            if (instance == null)
            {
                Debug.WriteLine(
                    "Không tạo được legend component.");
                return;
            }

            if (DebugMode)
            {
                _debugMarker.DrawCross(position, 0.01);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private void SetupTextType()
    {
        Element element = _doc.GetElement(_textTypeId);
        if (element is not TextNoteType type) return;

        Parameter size = type.get_Parameter(BuiltInParameter.TEXT_SIZE);
        if (size != null && !size.IsReadOnly)
        {
            size.Set(TableUnit.MmToFeet(TextHeightMm));
        }
    }

    private ElementId FindTextType()
    {
        FilteredElementCollector collector = new FilteredElementCollector(_doc)
            .OfClass(typeof(TextNoteType));

        foreach (TextNoteType type in collector)
        {
            return type.Id;
        }

        throw new InvalidOperationException("Không tìm thấy TextNoteType.");
    }

    private HorizontalTextAlignment ConvertHorizontalAlignment(
        TableHorizontalAlignment alignment)
    {
        return alignment switch
        {
            TableHorizontalAlignment.Left =>
                HorizontalTextAlignment.Left,

            TableHorizontalAlignment.Right =>
                HorizontalTextAlignment.Right,

            TableHorizontalAlignment.Center =>
                HorizontalTextAlignment.Center,

            _ =>
                HorizontalTextAlignment.Left
        };
    }

    private VerticalTextAlignment ConvertVerticalAlignment(
        TableVerticalAlignment alignment)
    {
        return alignment switch
        {
            TableVerticalAlignment.Top =>
                VerticalTextAlignment.Top,

            TableVerticalAlignment.Bottom =>
                VerticalTextAlignment.Bottom,

            TableVerticalAlignment.Middle =>
                VerticalTextAlignment.Middle,

            _ =>
                VerticalTextAlignment.Middle
        };
    }
}