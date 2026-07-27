using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using LongTool.Tables.Engine;
using LongTool.Tables.Renderer;
using LongTool.Tables.Builders;
using LongTool.Tables.Models;
using RevitView = Autodesk.Revit.DB.View;
using RevitTaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace LongTool.Tables.Commands;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class TableRenderCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        UIApplication uiapp = commandData.Application;
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Document doc = uidoc.Document;

        if (doc == null)
        {
            message = "Không có Document nào đang mở.";
            return Result.Failed;
        }

        // SỬA: Đổi tên biến từ RevitView thành currentView (chữ thường)
        RevitView currentView = doc.ActiveView;
        if (currentView == null)
        {
            message = "Không có View nào đang active.";
            return Result.Failed;
        }

        if (!(currentView is ViewPlan || currentView is ViewSection || currentView is ViewDrafting))
        {
            message = "View hiện tại không hỗ trợ vẽ DetailCurve. Hãy chọn ViewPlan, ViewSection hoặc ViewDrafting.";
            return Result.Failed;
        }

        try
        {
            Debug.WriteLine("===== BẮT ĐẦU TABLE RENDER COMMAND =====");

            // =====================
            // 1. TẠO BẢNG CHÍNH
            // =====================
            Debug.WriteLine("Đang tạo bảng chính...");
            Table mainTable = SampleTableBuilder.Create();
            Debug.WriteLine($"✅ Đã tạo bảng chính: {mainTable.RowCount} rows, {mainTable.ColumnCount} columns");

            // Debug thông tin bảng
            TableDebugHelper.DebugTableStructure(mainTable);
            TableDebugHelper.DebugTableContent(mainTable);
            TableDebugHelper.PrintIdMap(mainTable);

            // =====================
            // 2. TẠO ID MAP
            // =====================
            var idMap = TableDebugHelper.BuildIdToPositionMap(mainTable);
            Debug.WriteLine($"✅ Đã tạo ID map với {idMap.Count} ô");

            // =====================
            // 3. BUILD LAYOUT BẢNG CHÍNH
            // =====================
            Debug.WriteLine("Đang build layout bảng chính...");
            TableLayout mainLayout = mainTable.CreateLayout();
            mainLayout.Build();
            Debug.WriteLine($"✅ Đã build layout chính: {mainLayout.Cells.Count} cells");

            // =====================
            // 4. XÁC ĐỊNH VỊ TRÍ
            // =====================
            XYZ origin = GetTableOrigin(uidoc, currentView);
            XYZ mainOrigin = origin;

            // =====================
            // 5. RENDER BẢNG CHÍNH
            // =====================
            Debug.WriteLine("Đang render bảng chính...");
            var mainContext = new RevitRenderContext(doc, currentView, mainOrigin);
            var mainRenderService = new TableRenderService(mainContext);
            mainRenderService.Render(mainTable);
            Debug.WriteLine("✅ Render bảng chính hoàn tất");

            // =====================
            // 6. TẠO VÀ RENDER BẢNG DEBUG
            // =====================
            Debug.WriteLine("Đang tạo bảng debug...");
            Table debugTable = TableDebugHelper.CreateDebugTable(mainTable);
            Debug.WriteLine($"✅ Đã tạo bảng debug: {debugTable.RowCount} rows, {debugTable.ColumnCount} columns");

            if (debugTable.RowCount > 0 && debugTable.ColumnCount > 0)
            {
                TableLayout debugLayout = debugTable.CreateLayout();
                debugLayout.Build();
                Debug.WriteLine($"✅ Đã build layout debug: {debugLayout.Cells.Count} cells");

                // Vị trí bảng debug (bên dưới bảng chính)
                double mainTableHeight = GetTableHeight(mainLayout);
                XYZ debugOrigin = new XYZ(
                    origin.X,
                    origin.Y - mainTableHeight - 30,
                    origin.Z);

                Debug.WriteLine("Đang render bảng debug...");
                var debugContext = new RevitRenderContext(doc, currentView, debugOrigin);
                var debugRenderService = new TableRenderService(debugContext);
                debugRenderService.Render(debugTable);
                Debug.WriteLine("✅ Render bảng debug hoàn tất");

                // Zoom vào cả 2 bảng
                ZoomToBothTables(uidoc, mainLayout, mainContext, debugLayout, debugContext);
            }
            else
            {
                // Chỉ zoom vào bảng chính
                ZoomToTable(uidoc, mainLayout, mainContext);
            }

            // =====================
            // 7. HIỂN THỊ THÔNG TIN
            // =====================
            TableDebugHelper.ShowIdMapInDialog(mainTable);

            // Export CSV (tùy chọn)
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string csvPath = Path.Combine(desktopPath, $"IDMap_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
                TableDebugHelper.ExportIdMapToCsv(mainTable, csvPath);
                Debug.WriteLine($"✅ Đã export ID map ra: {csvPath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Không thể export CSV: {ex.Message}");
            }

            // =====================
            // 8. HOÀN TẤT
            // =====================
            Debug.WriteLine($"===== KẾT THÚC TABLE RENDER COMMAND =====");
            Debug.WriteLine($"✅ Đã render bảng chính {mainTable.RowCount}x{mainTable.ColumnCount}");
            Debug.WriteLine($"✅ Đã render bảng debug {debugTable.RowCount}x{debugTable.ColumnCount}");

            RevitTaskDialog.Show("Thành công",
                $"Đã render bảng thành công!\n" +
                $"Bảng chính: {mainTable.RowCount} hàng x {mainTable.ColumnCount} cột\n" +
                $"Bảng debug: {debugTable.RowCount} hàng x {debugTable.ColumnCount} cột\n" +
                $"Tổng số ô có ID: {idMap.Count}\n\n" +
                $"ID Map đã được export ra Desktop.");

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ LỖI: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            message = $"Lỗi: {ex.Message}";

            RevitTaskDialog.Show("Lỗi", $"Đã xảy ra lỗi:\n{ex.Message}\n\nChi tiết: {ex.StackTrace}");
            return Result.Failed;
        }
    }

    // SỬA: Đổi tên tham số từ RevitView thành view
    private XYZ GetTableOrigin(UIDocument uidoc, RevitView view)
    {
        BoundingBoxXYZ boundingBox = view.get_BoundingBox(null);
        if (boundingBox != null)
        {
            XYZ center = (boundingBox.Max + boundingBox.Min) / 2.0;
            return new XYZ(center.X - 20, center.Y + 10, 0);
        }
        return new XYZ(0, 0, 0);
    }

    private double GetTableHeight(TableLayout layout)
    {
        if (layout == null || layout.Cells.Count == 0) return 50;
        double maxY = double.MinValue;
        double minY = double.MaxValue;
        foreach (var cell in layout.Cells)
        {
            minY = Math.Min(minY, cell.Bottom);
            maxY = Math.Max(maxY, cell.Top);
        }
        return maxY - minY;
    }

    private void ZoomToTable(UIDocument uidoc, TableLayout layout, RevitRenderContext context)
    {
        if (layout == null || layout.Cells.Count == 0) return;

        try
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (var cell in layout.Cells)
            {
                XYZ bottomLeft = context.ToXYZ(cell.BottomLeft);
                XYZ topRight = context.ToXYZ(cell.TopRight);
                minX = Math.Min(minX, bottomLeft.X);
                minY = Math.Min(minY, bottomLeft.Y);
                maxX = Math.Max(maxX, topRight.X);
                maxY = Math.Max(maxY, topRight.Y);
            }

            double padding = 10;
            minX -= padding;
            minY -= padding;
            maxX += padding;
            maxY += padding;

            var bbox = new BoundingBoxXYZ();
            bbox.Min = new XYZ(minX, minY, 0);
            bbox.Max = new XYZ(maxX, maxY, 0);

            var uiViews = uidoc.GetOpenUIViews();
            if (uiViews != null && uiViews.Count > 0)
            {
                UIView uiView = uiViews.First();
                uiView.ZoomAndCenterRectangle(bbox.Min, bbox.Max);
                Debug.WriteLine("✅ Đã zoom vào bảng");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ Không thể zoom: {ex.Message}");
        }
    }

    private void ZoomToBothTables(UIDocument uidoc, TableLayout mainLayout, RevitRenderContext mainContext,
                                   TableLayout debugLayout, RevitRenderContext debugContext)
    {
        try
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            // Lấy bounds của bảng chính
            if (mainLayout != null && mainLayout.Cells.Count > 0)
            {
                foreach (var cell in mainLayout.Cells)
                {
                    XYZ bottomLeft = mainContext.ToXYZ(cell.BottomLeft);
                    XYZ topRight = mainContext.ToXYZ(cell.TopRight);
                    minX = Math.Min(minX, bottomLeft.X);
                    minY = Math.Min(minY, bottomLeft.Y);
                    maxX = Math.Max(maxX, topRight.X);
                    maxY = Math.Max(maxY, topRight.Y);
                }
            }

            // Lấy bounds của bảng debug
            if (debugLayout != null && debugLayout.Cells.Count > 0)
            {
                foreach (var cell in debugLayout.Cells)
                {
                    XYZ bottomLeft = debugContext.ToXYZ(cell.BottomLeft);
                    XYZ topRight = debugContext.ToXYZ(cell.TopRight);
                    minX = Math.Min(minX, bottomLeft.X);
                    minY = Math.Min(minY, bottomLeft.Y);
                    maxX = Math.Max(maxX, topRight.X);
                    maxY = Math.Max(maxY, topRight.Y);
                }
            }

            double padding = 20;
            minX -= padding;
            minY -= padding;
            maxX += padding;
            maxY += padding;

            var bbox = new BoundingBoxXYZ();
            bbox.Min = new XYZ(minX, minY, 0);
            bbox.Max = new XYZ(maxX, maxY, 0);

            var uiViews = uidoc.GetOpenUIViews();
            if (uiViews != null && uiViews.Count > 0)
            {
                UIView uiView = uiViews.First();
                uiView.ZoomAndCenterRectangle(bbox.Min, bbox.Max);
                Debug.WriteLine("✅ Đã zoom vào cả 2 bảng");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ Không thể zoom: {ex.Message}");
        }
    }
}