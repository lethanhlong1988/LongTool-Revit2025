using Autodesk.Revit.DB;
using LongTool.Utils;
using System;

namespace LongTool.Renderers.Table;

public class TableRenderer
{
    private readonly Document _document;
    private readonly View _view;

    public TableRenderer(
        Document document,
        View view)
    {
        _document = document ?? throw new ArgumentNullException(nameof(document));
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    /// <summary>
    /// Vẽ hình vuông với kích thước nhập vào theo đơn vị mm
    /// </summary>
    /// <param name="sizeInMm">Kích thước cạnh hình vuông (mm)</param>
    public void DrawSquare(double sizeInMm)
    {
        // Kiểm tra view có hợp lệ không
        if (_view == null || !_view.IsValidObject)
        {
            throw new InvalidOperationException("View is not valid.");
        }

        // Kiểm tra document
        if (_document == null || !_document.IsValidObject)
        {
            throw new InvalidOperationException("Document is not valid.");
        }

        // Chuyển đổi từ mm sang feet (Revit Internal Units)
        double sizeInFeet = UnitUtilsEx.MmToInternal(sizeInMm);
        double half = sizeInFeet / 2.0;

        // Tạo 4 điểm của hình vuông (trong hệ tọa độ feet)
        XYZ p1 = new XYZ(-half, -half, 0);
        XYZ p2 = new XYZ(half, -half, 0);
        XYZ p3 = new XYZ(half, half, 0);
        XYZ p4 = new XYZ(-half, half, 0);

        // Vẽ 4 cạnh
        DrawLine(p1, p2);
        DrawLine(p2, p3);
        DrawLine(p3, p4);
        DrawLine(p4, p1);
    }

    /// <summary>
    /// Vẽ hình vuông với kích thước nhập vào theo đơn vị mm
    /// Và có thể tùy chỉnh vị trí tâm
    /// </summary>
    /// <param name="sizeInMm">Kích thước cạnh hình vuông (mm)</param>
    /// <param name="centerX">Tọa độ X của tâm (mm)</param>
    /// <param name="centerY">Tọa độ Y của tâm (mm)</param>
    public void DrawSquare(double sizeInMm, double centerX, double centerY)
    {
        // Kiểm tra view có hợp lệ không
        if (_view == null || !_view.IsValidObject)
        {
            throw new InvalidOperationException("View is not valid.");
        }

        // Kiểm tra document
        if (_document == null || !_document.IsValidObject)
        {
            throw new InvalidOperationException("Document is not valid.");
        }

        // Chuyển đổi từ mm sang feet (Revit Internal Units)
        double sizeInFeet = UnitUtilsEx.MmToInternal(sizeInMm);
        double half = sizeInFeet / 2.0;

        // Chuyển đổi tọa độ tâm từ mm sang feet
        double centerXInFeet = UnitUtilsEx.MmToInternal(centerX);
        double centerYInFeet = UnitUtilsEx.MmToInternal(centerY);

        // Tạo 4 điểm của hình vuông với tâm tại (centerX, centerY)
        XYZ p1 = new XYZ(centerXInFeet - half, centerYInFeet - half, 0);
        XYZ p2 = new XYZ(centerXInFeet + half, centerYInFeet - half, 0);
        XYZ p3 = new XYZ(centerXInFeet + half, centerYInFeet + half, 0);
        XYZ p4 = new XYZ(centerXInFeet - half, centerYInFeet + half, 0);

        // Vẽ 4 cạnh
        DrawLine(p1, p2);
        DrawLine(p2, p3);
        DrawLine(p3, p4);
        DrawLine(p4, p1);
    }

    private void DrawLine(
        XYZ start,
        XYZ end)
    {
        // Kiểm tra điểm hợp lệ
        if (start == null || end == null)
        {
            throw new ArgumentNullException("Start or end point is null.");
        }

        // Kiểm tra khoảng cách giữa 2 điểm
        double distance = start.DistanceTo(end);
        if (distance < 0.0001)
        {
            throw new InvalidOperationException("Line length is too small.");
        }

        // Tạo Line
        Line line = Line.CreateBound(start, end);

        // Tạo Detail Curve
        DetailCurve detailCurve = _document.Create.NewDetailCurve(_view, line);

        if (detailCurve == null || !detailCurve.IsValidObject)
        {
            throw new InvalidOperationException("Failed to create detail curve.");
        }
    }
}