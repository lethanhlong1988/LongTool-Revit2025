using Autodesk.Revit.DB;
using LongTool.Tables.Geometry;
using System;

namespace LongTool.Tables.Revit;

/// <summary>
/// Chứa thông tin chung phục vụ việc render trong Revit.
/// </summary>
public sealed class RevitRenderContext
{
    #region Core

    public Document Document { get; }

    public View View { get; }

    /// <summary>
    /// Điểm gốc của bảng trong Revit.
    /// </summary>
    public XYZ Origin { get; }

    #endregion


    private const double MmToFeet = 1.0 / 304.8;
    private const double FeetToMm = 304.8;


    #region Constructor

    public RevitRenderContext(
        Document document,
        View view,
        XYZ origin)
    {
        Document = document
            ?? throw new ArgumentNullException(nameof(document));

        View = view
            ?? throw new ArgumentNullException(nameof(view));

        Origin = origin
            ?? throw new ArgumentNullException(nameof(origin));
    }

    #endregion



    #region Coordinate Conversion

    /// <summary>
    /// Chuyển TablePoint (mm) sang tọa độ Revit (feet).
    /// Đảo chiều Y vì TableEngine tính từ trên xuống,
    /// còn Revit tăng Y theo hướng lên.
    /// </summary>
    public XYZ ToXYZ(TablePoint point)
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point));


        return new XYZ(
            Origin.X + point.X * MmToFeet,
            Origin.Y - point.Y * MmToFeet,
            Origin.Z);
    }

    /// <summary>
    /// Chuyển tọa độ Revit (feet) sang TablePoint (mm).
    /// </summary>
    public TablePoint ToTablePoint(XYZ point)
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point));


        return new TablePoint(
            (point.X - Origin.X) * FeetToMm,
            (Origin.Y - point.Y) * FeetToMm);
    }

    #endregion
}