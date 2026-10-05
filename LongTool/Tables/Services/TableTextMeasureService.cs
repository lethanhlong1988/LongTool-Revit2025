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

        ValidateView();
        ValidateTextType();

        bool transactionStarted = false;
        Transaction? transaction = null;

        try
        {
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

            BoundingBoxXYZ? box =
                note.get_BoundingBox(_view);

            if (box == null)
            {
                DeleteTemporaryNote(note);

                CommitTransaction(
                    transaction,
                    transactionStarted);

                return TextMeasureResult.Empty;
            }

            double width =
                TableUnit.FeetToMm(
                    box.Max.X - box.Min.X);

            double height =
                TableUnit.FeetToMm(
                    box.Max.Y - box.Min.Y);

            DeleteTemporaryNote(note);

            CommitTransaction(
                transaction,
                transactionStarted);

            Debug.WriteLine(
                $"MEASURE RETURN " +
                $"Width(mm)={width}, " +
                $"Height(mm)={height}");

            return new TextMeasureResult(
                width,
                height);
        }
        catch
        {
            RollBackTransaction(
                transaction,
                transactionStarted);

            throw;
        }
    }

    // ==================================================
    // VALIDATION
    // ==================================================

    private void ValidateView()
    {
        if (_view.IsValidObject == false)
        {
            throw new InvalidOperationException(
                "View dùng để đo Text không còn hợp lệ.");
        }

        Element? viewElement =
            _doc.GetElement(_view.Id);

        if (viewElement == null)
        {
            throw new InvalidOperationException(
                "Không tìm thấy View trong Document.");
        }
    }

    private void ValidateTextType()
    {
        if (_textTypeId == ElementId.InvalidElementId)
        {
            throw new InvalidOperationException(
                "TextNoteTypeId không hợp lệ.");
        }

        Element? textType =
            _doc.GetElement(_textTypeId);

        if (textType is not TextNoteType)
        {
            throw new InvalidOperationException(
                "TextNoteType không còn tồn tại hoặc không hợp lệ.");
        }
    }

    // ==================================================
    // TEMPORARY NOTE
    // ==================================================

    private void DeleteTemporaryNote(
        TextNote note)
    {
        if (!note.IsValidObject)
            return;

        ElementId noteId =
            note.Id;

        if (noteId == ElementId.InvalidElementId)
            return;

        _doc.Delete(noteId);
    }

    // ==================================================
    // TRANSACTION
    // ==================================================

    private void CommitTransaction(
        Transaction? transaction,
        bool transactionStarted)
    {
        if (!transactionStarted || transaction == null)
            return;

        transaction.Commit();
    }

    private void RollBackTransaction(
        Transaction? transaction,
        bool transactionStarted)
    {
        if (!transactionStarted || transaction == null)
            return;

        try
        {
            if (transaction.GetStatus() == TransactionStatus.Started)
            {
                transaction.RollBack();
            }
        }
        catch
        {
            // Không che mất exception gốc.
        }
    }
}