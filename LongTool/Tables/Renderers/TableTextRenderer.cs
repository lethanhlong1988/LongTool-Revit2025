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

    private const double MmToFeet = 1.0 / 304.8;
    private const double TextHeightMm = 2.5;

    public TableTextRenderer(Document doc, View view, RevitRenderContext context)
    {
        _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _context = context ?? throw new ArgumentNullException(nameof(context));

        _textTypeId = FindTextType();
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

            //note.HorizontalAlignment = HorizontalTextAlignment.Center;
            //note.VerticalAlignment = VerticalTextAlignment.Middle;

            note.HorizontalAlignment =
                ConvertHorizontalAlignment(
                    cellLayout.Style.HorizontalAlignment);


            note.VerticalAlignment =
                ConvertVerticalAlignment(
                    cellLayout.Style.VerticalAlignment);

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