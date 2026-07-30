using Autodesk.Revit.DB;
using LongTool.Tables.Layout;
using System;
using System.Diagnostics;

namespace LongTool.Tables.Renderers;

public class TableTextRenderer
{
    private readonly Document _doc;
    private readonly View _view;

    private ElementId _textTypeId;


    private const double MmToFeet = 1.0 / 304.8;


    // Text height 3.5mm
    private const double TextHeightMm = 3.5;



    public TableTextRenderer(
        Document doc,
        View view)
    {
        _doc = doc ??
            throw new ArgumentNullException(nameof(doc));

        _view = view ??
            throw new ArgumentNullException(nameof(view));


        _textTypeId =
            FindTextType();


        SetupTextType();
    }





    public void Render(TableLayout layout)
    {
        Debug.WriteLine(
            "===== TEXT RENDERER START =====");


        foreach (var cellLayout in layout.Cells)
        {
            RenderCell(cellLayout);
        }


        Debug.WriteLine(
            "===== TEXT RENDERER END =====");
    }





    private void RenderCell(
        TableCellLayout cellLayout)
    {
        string text =
            cellLayout.Cell.Text;


        if (string.IsNullOrWhiteSpace(text))
            return;



        Debug.WriteLine(
            $"TEXT: {cellLayout.Cell.Name} -> {text}");



        XYZ position =
            new XYZ(
                cellLayout.CenterX * MmToFeet + 5,
                cellLayout.CenterY * MmToFeet + 5,
                0);



        TextNote note =
            TextNote.Create(
                _doc,
                _view.Id,
                position,
                text,
                _textTypeId);



        note.HorizontalAlignment =
            HorizontalTextAlignment.Center;
    }





    private void SetupTextType()
    {
        Element element =
            _doc.GetElement(_textTypeId);


        if (element is not TextNoteType type)
            return;



        Parameter size =
            type.get_Parameter(
                BuiltInParameter.TEXT_SIZE);



        if (size != null &&
            !size.IsReadOnly)
        {
            size.Set(
                TextHeightMm * MmToFeet);
        }
    }





    private ElementId FindTextType()
    {
        FilteredElementCollector collector =
            new FilteredElementCollector(_doc)
            .OfClass(typeof(TextNoteType));


        foreach (TextNoteType type in collector)
        {
            return type.Id;
        }


        throw new InvalidOperationException(
            "Không tìm thấy TextNoteType.");
    }
}