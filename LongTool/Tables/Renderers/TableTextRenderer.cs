using Autodesk.Revit.DB;
using LongTool.Tables.Geometry;
using LongTool.Tables.Layout;
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
    private ElementId _textTypeId;
    private readonly TextMetricsService _textMetrics;

    private const double MmToFeet = 1.0 / 304.8;
    private const double TextHeightMm = 5;

    // ✅ Sửa constructor để nhận context
    public TableTextRenderer(Document doc, View view, RevitRenderContext context)
    {
        _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _context = context ?? throw new ArgumentNullException(nameof(context));

        _textTypeId = FindTextType();

        _textMetrics =
            new TextMetricsService(_doc);

        SetupTextType();
    }

    public void Render(TableLayout layout)
    {
        Debug.WriteLine("===== TEXT RENDERER START =====");

        foreach (var cellLayout in layout.Cells)
        {
            RenderCell(cellLayout);
        }

        Debug.WriteLine("===== TEXT RENDERER END =====");
    }

    private void RenderCell(
    TableCellLayout cellLayout)
    {
        const string text = "ABC";

        try
        {
            XYZ position =
                _context.ToXYZ(
                    cellLayout.TextAnchor);


            TextNote note =
                TextNote.Create(
                    _doc,
                    _view.Id,
                    position,
                    text,
                    _textTypeId);


            if (note == null)
                return;


            note.HorizontalAlignment =
                HorizontalTextAlignment.Center;


            double offset =
                _textMetrics.GetVerticalOffset(
                    note,
                    _view);


            if (Math.Abs(offset) > 1e-9)
            {
                ElementTransformUtils.MoveElement(
                    _doc,
                    note.Id,
                    new XYZ(
                        0,
                        offset,
                        0));
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
            size.Set(TextHeightMm * MmToFeet);
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
}