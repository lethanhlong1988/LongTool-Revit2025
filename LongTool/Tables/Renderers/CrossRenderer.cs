using Autodesk.Revit.DB;
using LongTool.Tables.Layout;
using LongTool.Tables.Models;
using LongTool.Tables.Revit;
using System;
using System.Diagnostics;  // ✅ Thêm using này

namespace LongTool.Tables.Rendering;

internal sealed class CrossRenderer
{
    private readonly RevitRenderContext _context;

    public CrossRenderer(RevitRenderContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Render(TableLayout layout)
    {
        if (layout == null)
            throw new ArgumentNullException(nameof(layout));

        if (!layout.IsBuilt)
            throw new InvalidOperationException("TableLayout chưa được Build().");

        // ✅ LOG 1: Kiểm tra số lượng Cells
        Debug.WriteLine($"===== CROSS RENDERER =====");
        Debug.WriteLine($"Số lượng Cells trong layout: {layout.Cells.Count}");

        int crossCount = 0;
        foreach (TableCellLayout cell in layout.Cells)
        {
            // ✅ LOG 2: Kiểm tra cell nào có HasCrossLine
            if (cell.Cell.HasCrossLine)
            {
                crossCount++;
                Debug.WriteLine($"✅ Tìm thấy cell có gạch chéo tại Row={cell.RowIndex}, Col={cell.ColumnIndex}");
                Debug.WriteLine($"   - Text: '{cell.Cell.Text}'");
                Debug.WriteLine($"   - Direction: {cell.Cell.CrossDirection}");
                Debug.WriteLine($"   - Position: ({cell.Left}, {cell.Bottom}) -> ({cell.Right}, {cell.Top})");
                RenderCrossLine(cell);
            }
        }

        if (crossCount == 0)
        {
            Debug.WriteLine($"⚠️ KHÔNG TÌM THẤY CELL NÀO CÓ HasCrossLine = true");
        }
        else
        {
            Debug.WriteLine($"Đã vẽ {crossCount} đường gạch chéo");
        }
        Debug.WriteLine($"===== KẾT THÚC CROSS RENDERER =====");
    }

    private void RenderCrossLine(TableCellLayout cell)
    {
        if (cell == null) return;
        if (!cell.Cell.HasCrossLine) return;

        try
        {
            XYZ start;
            XYZ end;

            switch (cell.Cell.CrossDirection)
            {
                case CrossLineDirection.TopLeftToBottomRight:
                    start = _context.ToXYZ(cell.TopLeft);
                    end = _context.ToXYZ(cell.BottomRight);
                    Debug.WriteLine($"   - Vẽ đường chéo / (TopLeft -> BottomRight)");
                    break;

                case CrossLineDirection.TopRightToBottomLeft:
                    start = _context.ToXYZ(cell.TopRight);
                    end = _context.ToXYZ(cell.BottomLeft);
                    Debug.WriteLine($"   - Vẽ đường chéo / (TopRight -> BottomLeft)");
                    break;

                case CrossLineDirection.BottomLeftToTopRight:
                    start = _context.ToXYZ(cell.BottomLeft);
                    end = _context.ToXYZ(cell.TopRight);
                    Debug.WriteLine($"   - Vẽ đường chéo / (BottomLeft -> TopRight)");
                    break;

                default:
                    return;
            }

            if (start.DistanceTo(end) < 0.0001)
            {
                Debug.WriteLine($"   ⚠️ Bỏ qua: khoảng cách quá nhỏ");
                return;
            }

            Debug.WriteLine($"   - Start: ({start.X}, {start.Y}, {start.Z})");
            Debug.WriteLine($"   - End: ({end.X}, {end.Y}, {end.Z})");

            Line geometryLine = Line.CreateBound(start, end);
            DetailCurve curve = _context.Document.Create.NewDetailCurve(
                _context.View, geometryLine);

            if (curve == null)
            {
                Debug.WriteLine($"   ❌ Không tạo được DetailCurve");
                return;
            }

            Debug.WriteLine($"   ✅ Đã tạo DetailCurve thành công");

            ApplyCrossLineStyle(curve);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Lỗi vẽ gạch chéo: {ex.Message}");
        }
    }

    private void ApplyCrossLineStyle(DetailCurve curve)
    {
        if (curve == null) return;

        try
        {
            OverrideGraphicSettings settings = new OverrideGraphicSettings();
            settings.SetProjectionLineWeight(2);
            Autodesk.Revit.DB.Color blackColor = new Autodesk.Revit.DB.Color(0, 0, 0);
            settings.SetProjectionLineColor(blackColor);
            _context.View.SetElementOverrides(curve.Id, settings);

            Debug.WriteLine($"   ✅ Đã áp dụng style cho đường gạch chéo");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Lỗi áp dụng style: {ex.Message}");
        }
    }
}