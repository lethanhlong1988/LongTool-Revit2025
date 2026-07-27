using Autodesk.Revit.DB;
using LongTool.Tables.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;  // ✅ Thêm using này

namespace LongTool.Tables.Renderer;

internal sealed class BorderRenderer
{
    private readonly RevitRenderContext _context;
    private readonly CrossRenderer _crossRenderer;

    public BorderRenderer(RevitRenderContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _crossRenderer = new CrossRenderer(context);
    }

    public void Render(TableLayout layout)
    {
        if (layout == null)
            throw new ArgumentNullException(nameof(layout));

        if (!layout.IsBuilt)
            throw new InvalidOperationException("TableLayout chưa được Build().");

        // ✅ LOG 1: Kiểm tra số lượng BorderLines
        Debug.WriteLine($"===== BORDER RENDERER =====");
        Debug.WriteLine($"Số lượng BorderLines: {layout.BorderLines.Count}");

        // Vẽ border
        int lineCount = 0;
        foreach (TableLine line in layout.BorderLines)
        {
            lineCount++;
            // ✅ LOG 2: In thông tin từng đường
            Debug.WriteLine($"  Line {lineCount}: {line.Start} -> {line.End}, Width: {line.LineWidth}");
            DrawLine(line);
        }
        Debug.WriteLine($"Đã vẽ {lineCount} đường viền");

        // ✅ LOG 3: Kiểm tra CrossRenderer
        Debug.WriteLine($"----- GỌI CROSS RENDERER -----");
        _crossRenderer.Render(layout);
        Debug.WriteLine($"===== KẾT THÚC BORDER RENDERER =====");
    }

    private void DrawLine(TableLine line)
    {
        if (line == null) return;

        XYZ start = _context.ToXYZ(line.Start);
        XYZ end = _context.ToXYZ(line.End);

        if (start.DistanceTo(end) < 0.0001) return;

        Line geometryLine = Line.CreateBound(start, end);
        DetailCurve curve = _context.Document.Create.NewDetailCurve(
            _context.View, geometryLine);

        if (curve == null) return;

        ApplyLineWeight(curve, line.LineWidth);
    }

    private void ApplyLineWeight(DetailCurve curve, double lineWidthMm)
    {
        if (curve == null) return;

        try
        {
            int lineWeight = ConvertMmToLineWeight(lineWidthMm);
            OverrideGraphicSettings settings = new OverrideGraphicSettings();
            settings.SetProjectionLineWeight(lineWeight);
            _context.View.SetElementOverrides(curve.Id, settings);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Lỗi apply LineWeight: {ex.Message}");
        }
    }

    private int ConvertMmToLineWeight(double mm)
    {
        if (mm <= 0.1) return 1;
        if (mm <= 0.15) return 2;
        if (mm <= 0.2) return 3;
        if (mm <= 0.25) return 4;
        if (mm <= 0.35) return 5;
        if (mm <= 0.5) return 6;
        if (mm <= 0.7) return 7;
        if (mm <= 1.0) return 8;
        if (mm <= 1.4) return 9;
        if (mm <= 2.0) return 10;
        if (mm <= 2.8) return 11;
        if (mm <= 4.0) return 12;
        if (mm <= 5.6) return 13;
        if (mm <= 8.0) return 14;
        if (mm <= 11.0) return 15;
        if (mm <= 16.0) return 16;
        if (mm <= 22.0) return 17;
        if (mm <= 32.0) return 18;
        if (mm <= 45.0) return 19;
        return 20;
    }
}