using Autodesk.Revit.DB;
using System;

namespace LongTool.Tables.Diagnostics;

/// <summary>
/// Công cụ hỗ trợ vẽ marker phục vụ debug hình học.
/// Không sử dụng trong chức năng chính.
/// </summary>
public sealed class DebugMarkerRenderer
{
    private readonly Document _doc;
    private readonly View _view;


    public DebugMarkerRenderer(
        Document doc,
        View view)
    {
        _doc =
            doc ??
            throw new ArgumentNullException(nameof(doc));

        _view =
            view ??
            throw new ArgumentNullException(nameof(view));
    }



    /// <summary>
    /// Vẽ dấu X tại một điểm.
    /// Đơn vị size: feet Revit.
    /// </summary>
    public void DrawCross(
        XYZ point,
        double size)
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point));


        XYZ p1 =
            new XYZ(
                point.X - size,
                point.Y - size,
                point.Z);


        XYZ p2 =
            new XYZ(
                point.X + size,
                point.Y + size,
                point.Z);


        XYZ p3 =
            new XYZ(
                point.X - size,
                point.Y + size,
                point.Z);


        XYZ p4 =
            new XYZ(
                point.X + size,
                point.Y - size,
                point.Z);



        DetailCurve line1 =
            CreateLine(
                p1,
                p2);


        DetailCurve line2 =
            CreateLine(
                p3,
                p4);
    }



    private DetailCurve CreateLine(
        XYZ start,
        XYZ end)
    {
        Line line =
            Line.CreateBound(
                start,
                end);


        return
            _doc.Create.NewDetailCurve(
                _view,
                line);
    }
}