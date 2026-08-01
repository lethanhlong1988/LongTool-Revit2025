using Autodesk.Revit.DB;
using System;

namespace LongTool.Tables.Services;

/// <summary>
/// Cung cấp các phép đo hình học của TextNote.
/// Không thực hiện render hay di chuyển Text.
/// </summary>
public sealed class TextMetricsService
{
    private readonly Document _document;

    public TextMetricsService(
        Document document)
    {
        _document =
            document ??
            throw new ArgumentNullException(nameof(document));
    }



    /// <summary>
    /// Tính khoảng lệch theo phương Y giữa
    /// điểm chèn TextNote và tâm BoundingBox.
    /// </summary>
    public double GetVerticalOffset(
        TextNote note,
        View view)
    {
        if (note == null)
            throw new ArgumentNullException(nameof(note));

        if (view == null)
            throw new ArgumentNullException(nameof(view));


        _document.Regenerate();


        BoundingBoxXYZ box =
            note.get_BoundingBox(view);

        if (box == null)
            return 0.0;


        XYZ center =
            (box.Min + box.Max) / 2.0;


        LocationPoint location =
            note.Location as LocationPoint;

        if (location == null)
            return 0.0;


        return
            location.Point.Y - center.Y;
    }
}