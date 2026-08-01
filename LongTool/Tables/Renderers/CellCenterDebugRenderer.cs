using Autodesk.Revit.DB;
using LongTool.Tables.Layout;
using LongTool.Tables.Revit;
using System;

namespace LongTool.Tables.Renderers;

/// <summary>
/// Renderer dùng để kiểm tra vị trí tâm của từng Cell.
/// Chỉ phục vụ Debug.
/// </summary>
public sealed class CellCenterDebugRenderer
{
    private readonly RevitRenderContext _context;

    private readonly Document _doc;
    private readonly View _view;

    public CellCenterDebugRenderer(
        RevitRenderContext context)
    {
        _context =
            context ??
            throw new ArgumentNullException(nameof(context));

        _doc = context.Document;
        _view = context.View;
    }



    public void Render(
        TableLayout layout)
    {
        foreach (TableCellLayout cell in layout.Cells)
        {
            CreateCenterText(cell);
        }
    }



    private void CreateCenterText(
        TableCellLayout cell)
    {
        XYZ point =
            _context.ToXYZ(
                cell.TextAnchor);


        TextNoteType type =
            GetTextNoteType();


        TextNote note =
            TextNote.Create(
                _doc,
                _view.Id,
                point,
                "X",
                type.Id);


        note.HorizontalAlignment =
            HorizontalTextAlignment.Center;
    }



    private TextNoteType GetTextNoteType()
    {
        foreach (Element element in
                 new FilteredElementCollector(_doc)
                     .OfClass(typeof(TextNoteType)))
        {
            if (element is TextNoteType type)
            {
                return type;
            }
        }

        throw new InvalidOperationException(
            "Không tìm thấy TextNoteType.");
    }
}