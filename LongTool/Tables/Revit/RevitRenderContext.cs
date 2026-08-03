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
    /// Chuyển TablePoint (mm) sang Revit tọa độ (feet).
    /// Lưu ý: TableEngine tính Y từ trên xuống, Revit tính Y từ dưới lên.
    /// </summary>
    public XYZ ToXYZ(TablePoint point)
    {
        return new XYZ(
            Origin.X + TableUnit.MmToFeet(point.X),
            Origin.Y + TableUnit.MmToFeet(point.Y),
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
            TableUnit.FeetToMm(point.X - Origin.X),
            TableUnit.FeetToMm(point.Y - Origin.Y));
    }

    #endregion
}