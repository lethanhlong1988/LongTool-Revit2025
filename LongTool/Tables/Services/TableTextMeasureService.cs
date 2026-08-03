using Autodesk.Revit.DB;
using LongTool.Tables.Geometry;
using System;
using System.Diagnostics;

namespace LongTool.Tables.Services;

public sealed class TableTextMeasureService
{
    private readonly Document _doc;
    private readonly View _view;
    private readonly ElementId _textTypeId;

    public TableTextMeasureService(
        Document doc,
        View view,
        ElementId textTypeId)
    {
        _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _textTypeId = textTypeId;
    }

    public TextMeasureResult Measure(
        string text,
        XYZ position)
    {
        if (string.IsNullOrWhiteSpace(text))
            return TextMeasureResult.Empty;

        bool transactionStarted = false;

        Transaction transaction = null;


        if (!_doc.IsModifiable)
        {
            transaction =
                new Transaction(
                    _doc,
                    "Measure Text");

            transaction.Start();

            transactionStarted = true;
        }

        TextNote note =
            TextNote.Create(
                _doc,
                _view.Id,
                position,
                text,
                _textTypeId);

        if (note == null)
            return TextMeasureResult.Empty;

        _doc.Regenerate();

        BoundingBoxXYZ box =
            note.get_BoundingBox(_view);

        if (box == null)
        {
            _doc.Delete(note.Id);

            if (transactionStarted)
            {
                transaction.Commit();
            }

            return TextMeasureResult.Empty;
        }

        double width =
            TableUnit.FeetToMm(
                box.Max.X - box.Min.X);

        double height =
            TableUnit.FeetToMm(
                box.Max.Y - box.Min.Y);

        // Xóa TextNote tạm
        _doc.Delete(note.Id);

        Debug.WriteLine(
            $"MEASURE RETURN Width(mm)={width}");

        return new TextMeasureResult(
            width,
            height);
    }
}