using Autodesk.Revit.DB;
using LongTool.Tables.Geometry;
using LongTool.Tables.Layout;
using LongTool.Tables.Revit;  // ✅ Thêm using này
using System;
using System.Diagnostics;

namespace LongTool.Tables.Renderers;

public class TableTextRenderer
{
    private readonly Document _doc;
    private readonly View _view;
    private readonly RevitRenderContext _context;  // ✅ Thêm context
    private ElementId _textTypeId;

    private const double MmToFeet = 1.0 / 304.8;
    private const double TextHeightMm = 3.5;

    // ✅ Sửa constructor để nhận context
    public TableTextRenderer(Document doc, View view, RevitRenderContext context)
    {
        _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _context = context ?? throw new ArgumentNullException(nameof(context));

        _textTypeId = FindTextType();
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

    private void RenderCell(TableCellLayout cellLayout)
    {
        string text = cellLayout.Cell.Text;
        if (string.IsNullOrWhiteSpace(text)) return;

        Debug.WriteLine($"TEXT: {cellLayout.Cell.Name} -> {text}");

        try
        {
            // ✅ Dùng ToXYZ của context để chuyển đổi đúng
            TablePoint centerPoint = new TablePoint(
                cellLayout.CenterX,
                cellLayout.CenterY
            );

            XYZ position = _context.ToXYZ(centerPoint);

            Debug.WriteLine($"  Table Center: ({cellLayout.CenterX}, {cellLayout.CenterY}) mm");
            Debug.WriteLine($"  Revit Position: ({position.X}, {position.Y}, {position.Z}) feet");

            TextNote note = TextNote.Create(
                _doc,
                _view.Id,
                position,
                text,
                _textTypeId);

            if (note != null)
            {
                note.HorizontalAlignment = HorizontalTextAlignment.Center;
                Debug.WriteLine($"  ✅ Created TextNote: {text}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"  ❌ Failed to create TextNote: {ex.Message}");
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