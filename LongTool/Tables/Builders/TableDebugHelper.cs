using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LongTool.Tables.Engine;
using Autodesk.Revit.UI;

// ✅ Tạo alias để tránh ambiguous reference
using MyTable = LongTool.Tables.Models.Table;
using MyTableCell = LongTool.Tables.Models.TableCell;
using LongTool.Tables.Models;

namespace LongTool.Tables.Builders;

public static class TableDebugHelper
{
    /// <summary>
    /// Tạo bảng debug hiển thị ID của từng ô
    /// </summary>
    public static MyTable CreateDebugTable(MyTable mainTable)
    {
        if (mainTable == null)
            throw new ArgumentNullException(nameof(mainTable));

        int rowCount = mainTable.RowCount;
        int colCount = mainTable.ColumnCount;

        Debug.WriteLine($"CreateDebugTable: {rowCount}x{colCount}");

        if (rowCount == 0 || colCount == 0)
        {
            Debug.WriteLine("⚠️ Bảng rỗng, không thể tạo bảng debug");
            return new MyTable();
        }

        MyTable debugTable = new MyTable();

        // Thêm columns với chiều rộng phù hợp
        for (int col = 0; col < colCount; col++)
        {
            double colWidth = 25;
            try
            {
                colWidth = Math.Max(mainTable.Columns[col]?.Width ?? 25, 25);
            }
            catch { }
            debugTable.AddColumn(colWidth);
        }

        // Duyệt qua từng row
        for (int row = 0; row < rowCount; row++)
        {
            double rowHeight = 10; // Tăng chiều cao để hiển thị đủ thông tin
            try
            {
                rowHeight = Math.Max(mainTable.Rows[row]?.Height ?? 10, 10);
            }
            catch { }

            TableRow debugRow = debugTable.AddRow(rowHeight);
            var mainRow = mainTable.Rows[row];

            // ✅ QUAN TRỌNG: Sử dụng biến colIndex để theo dõi vị trí cột thực tế
            int colIndex = 0;

            // Duyệt qua từng cell trong row (không phải từng cột)
            foreach (MyTableCell mainCell in mainRow)
            {
                try
                {
                    if (mainCell != null)
                    {
                        // Lấy thông tin của ô
                        string cellId = mainCell.Id ?? $"R{row}C{colIndex}";
                        string cellText = mainCell.Text ?? "";
                        string cellName = mainCell.Name ?? "";
                        int colSpan = mainCell.ColSpan;
                        int rowSpan = mainCell.RowSpan;

                        // Tạo text hiển thị cho ô debug
                        string displayText = $"ID: {cellId}";
                        if (!string.IsNullOrEmpty(cellName))
                        {
                            displayText += $"\nName: {cellName}";
                        }
                        if (!string.IsNullOrEmpty(cellText))
                        {
                            displayText += $"\nText: {cellText}";
                        }
                        displayText += $"\nSpan: {colSpan}x{rowSpan}";

                        // ✅ Thêm cell vào debugRow
                        MyTableCell debugCell = debugRow.AddCell();
                        debugCell.Text = displayText;
                        debugCell.Name = $"Debug_{cellId}";

                        // ✅ QUAN TRỌNG: Copy ColSpan và RowSpan từ cell gốc
                        debugCell.ColSpan = colSpan;
                        debugCell.RowSpan = rowSpan;

                        // Style cho cell debug
                        debugCell.Style.Borders.Top = true;
                        debugCell.Style.Borders.Bottom = true;
                        debugCell.Style.Borders.Left = true;
                        debugCell.Style.Borders.Right = true;
                        debugCell.Style.HorizontalAlignment = TableHorizontalAlignment.Center;
                        debugCell.Style.VerticalAlignment = TableVerticalAlignment.Middle;
                        debugCell.Style.Text.FontSize = 2.5;

                        // Highlight ô có Name
                        if (!string.IsNullOrEmpty(cellName))
                        {
                            debugCell.Style.Text.Bold = true;
                        }

                        Debug.WriteLine($"  ✅ Debug Cell [{row},{colIndex}]: {cellId}, Span: {colSpan}x{rowSpan}");

                        // ✅ Cập nhật colIndex dựa trên ColSpan của cell gốc
                        colIndex += colSpan;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"⚠️ Lỗi tại ô row {row}, col {colIndex}: {ex.Message}");
                    try
                    {
                        MyTableCell debugCell = debugRow.AddCell();
                        debugCell.Text = $"ERR: {ex.Message}";
                        colIndex++;
                    }
                    catch { }
                }
            }
        }

        Debug.WriteLine($"✅ Đã tạo bảng debug: {debugTable.RowCount}x{debugTable.ColumnCount}");
        return debugTable;
    }

    /// <summary>
    /// Tạo bảng debug chi tiết
    /// </summary>
    public static MyTable CreateDetailedDebugTable(MyTable mainTable)
    {
        if (mainTable == null)
            throw new ArgumentNullException(nameof(mainTable));

        int rowCount = mainTable.RowCount;
        int colCount = mainTable.ColumnCount;

        Debug.WriteLine($"CreateDetailedDebugTable: {rowCount}x{colCount}");

        if (rowCount == 0 || colCount == 0)
        {
            Debug.WriteLine("⚠️ Bảng rỗng, không thể tạo bảng debug");
            return new MyTable();
        }

        MyTable debugTable = new MyTable();

        for (int col = 0; col < colCount; col++)
        {
            double colWidth = 30;
            try
            {
                colWidth = Math.Max(mainTable.Columns[col]?.Width ?? 30, 30);
            }
            catch { }
            debugTable.AddColumn(colWidth);
        }

        // Header row
        TableRow headerRow = debugTable.AddRow(6);
        for (int col = 0; col < colCount; col++)
        {
            MyTableCell headerCell = headerRow.AddCell();
            headerCell.Text = $"Col {col}";
            headerCell.Style.HorizontalAlignment = TableHorizontalAlignment.Center;
            headerCell.Style.VerticalAlignment = TableVerticalAlignment.Middle;
            headerCell.Style.Text.Bold = true;
            headerCell.Style.Text.FontSize = 3.0;
            headerCell.Style.Borders.Top = true;
            headerCell.Style.Borders.Bottom = true;
            headerCell.Style.Borders.Left = true;
            headerCell.Style.Borders.Right = true;
        }

        for (int row = 0; row < rowCount; row++)
        {
            TableRow debugRow = debugTable.AddRow(8);
            var mainRow = mainTable.Rows[row];

            // Row number
            MyTableCell rowNumCell = debugRow.AddCell();
            rowNumCell.Text = $"Row {row}";
            rowNumCell.Style.HorizontalAlignment = TableHorizontalAlignment.Center;
            rowNumCell.Style.VerticalAlignment = TableVerticalAlignment.Middle;
            rowNumCell.Style.Text.Bold = true;
            rowNumCell.Style.Text.FontSize = 2.5;
            rowNumCell.Style.Borders.Top = true;
            rowNumCell.Style.Borders.Bottom = true;
            rowNumCell.Style.Borders.Left = true;
            rowNumCell.Style.Borders.Right = true;

            for (int col = 0; col < colCount; col++)
            {
                try
                {
                    MyTableCell mainCell = mainRow[col];
                    if (mainCell != null)
                    {
                        string cellId = mainCell.Id ?? $"R{row}C{col}";
                        string cellText = mainCell.Text ?? "";
                        string cellName = mainCell.Name ?? "";

                        string displayText = $"ID: {cellId}";
                        if (!string.IsNullOrEmpty(cellName))
                            displayText += $"\nName: {cellName}";
                        if (!string.IsNullOrEmpty(cellText))
                            displayText += $"\nText: {cellText}";

                        MyTableCell debugCell = debugRow.AddCell();
                        debugCell.Text = displayText;
                        debugCell.Name = $"Debug_{cellId}";

                        debugCell.Style.HorizontalAlignment = TableHorizontalAlignment.Center;
                        debugCell.Style.VerticalAlignment = TableVerticalAlignment.Middle;
                        debugCell.Style.Text.FontSize = 2.5;
                        debugCell.Style.Borders.Top = true;
                        debugCell.Style.Borders.Bottom = true;
                        debugCell.Style.Borders.Left = true;
                        debugCell.Style.Borders.Right = true;
                    }
                    else
                    {
                        MyTableCell debugCell = debugRow.AddCell();
                        debugCell.Text = $"NULL";
                        debugCell.Style.Borders.Top = true;
                        debugCell.Style.Borders.Bottom = true;
                        debugCell.Style.Borders.Left = true;
                        debugCell.Style.Borders.Right = true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"⚠️ Lỗi tại ô [{row},{col}]: {ex.Message}");
                    try
                    {
                        MyTableCell debugCell = debugRow.AddCell();
                        debugCell.Text = $"ERR";
                    }
                    catch { }
                }
            }
        }

        return debugTable;
    }

    /// <summary>
    /// Debug thông tin cấu trúc của Table
    /// </summary>
    public static void DebugTableStructure(MyTable table)
    {
        if (table == null)
        {
            Debug.WriteLine("Table is null");
            return;
        }

        Debug.WriteLine($"=== TABLE STRUCTURE ===");
        Debug.WriteLine($"RowCount: {table.RowCount}");
        Debug.WriteLine($"ColumnCount: {table.ColumnCount}");

        Debug.WriteLine($"Columns:");
        for (int col = 0; col < table.ColumnCount; col++)
        {
            try
            {
                var column = table.Columns[col];
                Debug.WriteLine($"  Col {col}: Width={column?.Width ?? 0}");
            }
            catch { }
        }

        Debug.WriteLine($"Rows:");
        for (int row = 0; row < Math.Min(table.RowCount, 5); row++)
        {
            try
            {
                var tableRow = table.Rows[row];
                Debug.WriteLine($"  Row {row}: Height={tableRow?.Height ?? 0}, Cells={tableRow?.Count ?? 0}");
            }
            catch { }
        }

        if (table.RowCount > 5)
            Debug.WriteLine($"  ... và {table.RowCount - 5} rows khác");

        Debug.WriteLine("=== END TABLE STRUCTURE ===");
    }

    /// <summary>
    /// Kiểm tra và in thông tin các cell trong bảng
    /// </summary>
    public static void DebugTableContent(MyTable table)
    {
        if (table == null)
        {
            Debug.WriteLine("Table is null");
            return;
        }

        Debug.WriteLine($"=== TABLE CONTENT ({table.RowCount}x{table.ColumnCount}) ===");

        for (int row = 0; row < table.RowCount; row++)
        {
            var tableRow = table.Rows[row];
            string rowStr = $"Row {row:D2} (H:{tableRow.Height:F1}): ";

            for (int col = 0; col < table.ColumnCount; col++)
            {
                try
                {
                    MyTableCell cell = tableRow[col];
                    if (cell != null)
                    {
                        string text = cell.Text ?? "";
                        if (text.Length > 15)
                            text = text.Substring(0, 12) + "...";
                        rowStr += $"[{text}] ";
                    }
                    else
                    {
                        rowStr += "[null] ";
                    }
                }
                catch
                {
                    rowStr += "[ERR] ";
                }
            }
            Debug.WriteLine(rowStr);
        }
        Debug.WriteLine("=== END TABLE CONTENT ===");
    }

    /// <summary>
    /// In ID map ra debug output
    /// </summary>
    public static void PrintIdMap(MyTable table)
    {
        if (table == null)
        {
            Debug.WriteLine("Table is null");
            return;
        }

        Debug.WriteLine($"=== ID MAP ({table.RowCount}x{table.ColumnCount}) ===");
        Debug.WriteLine($"{"Row",-5} {"Col",-5} {"ID",-15} {"Name",-20} {"Text"}");
        Debug.WriteLine(new string('-', 70));

        for (int row = 0; row < table.RowCount; row++)
        {
            var tableRow = table.Rows[row];
            for (int col = 0; col < table.ColumnCount; col++)
            {
                try
                {
                    MyTableCell cell = tableRow[col];
                    if (cell != null)
                    {
                        string id = cell.Id ?? "null";
                        string name = cell.Name ?? "";
                        string text = cell.Text ?? "";

                        if (text.Length > 30)
                            text = text.Substring(0, 27) + "...";

                        Debug.WriteLine($"{row,-5} {col,-5} {id,-15} {name,-20} {text}");
                    }
                }
                catch { }
            }
        }
        Debug.WriteLine("=== END ID MAP ===");
    }

    /// <summary>
    /// Build ID map từ bảng
    /// </summary>
    public static Dictionary<string, (int Row, int Col)> BuildIdToPositionMap(MyTable table)
    {
        var map = new Dictionary<string, (int Row, int Col)>();

        if (table == null || table.RowCount == 0 || table.ColumnCount == 0)
            return map;

        for (int row = 0; row < table.RowCount; row++)
        {
            var tableRow = table.Rows[row];
            for (int col = 0; col < table.ColumnCount; col++)
            {
                try
                {
                    MyTableCell cell = tableRow[col];
                    if (cell != null && !string.IsNullOrEmpty(cell.Id))
                    {
                        map[cell.Id] = (row, col);
                    }
                }
                catch { }
            }
        }

        return map;
    }

    /// <summary>
    /// Tìm cell theo ID
    /// </summary>
    public static (int Row, int Col)? FindCellById(MyTable table, string id)
    {
        if (table == null || string.IsNullOrEmpty(id))
            return null;

        for (int row = 0; row < table.RowCount; row++)
        {
            var tableRow = table.Rows[row];
            for (int col = 0; col < table.ColumnCount; col++)
            {
                try
                {
                    MyTableCell cell = tableRow[col];
                    if (cell != null && cell.Id == id)
                    {
                        return (row, col);
                    }
                }
                catch { }
            }
        }

        return null;
    }

    /// <summary>
    /// Export ID map ra file CSV
    /// </summary>
    public static void ExportIdMapToCsv(MyTable table, string filePath)
    {
        if (table == null || table.RowCount == 0 || table.ColumnCount == 0)
        {
            Debug.WriteLine("⚠️ Không có dữ liệu để export");
            return;
        }

        try
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Row,Col,Id,Name,Text,ColSpan,RowSpan,HasCrossLine");

                for (int row = 0; row < table.RowCount; row++)
                {
                    var tableRow = table.Rows[row];
                    for (int col = 0; col < table.ColumnCount; col++)
                    {
                        try
                        {
                            MyTableCell cell = tableRow[col];
                            if (cell != null)
                            {
                                string text = cell.Text?.Replace(",", ";") ?? "";
                                string name = cell.Name?.Replace(",", ";") ?? "";

                                writer.WriteLine($"{row},{col},{cell.Id},{name},{text},{cell.ColSpan},{cell.RowSpan},{cell.HasCrossLine}");
                            }
                        }
                        catch { }
                    }
                }
            }

            Debug.WriteLine($"✅ Đã export ID map ra file: {filePath}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Lỗi export CSV: {ex.Message}");
        }
    }

    /// <summary>
    /// Hiển thị thông tin ID map trong TaskDialog
    /// </summary>
    public static void ShowIdMapInDialog(MyTable table)
    {
        if (table == null || table.RowCount == 0 || table.ColumnCount == 0)
        {
            TaskDialog.Show("ID Map", "Không có dữ liệu ID map");
            return;
        }

        string message = "=== ID MAP ===\n";
        message += $"Tổng số ô: {table.RowCount * table.ColumnCount}\n";
        message += $"Số hàng: {table.RowCount}\n";
        message += $"Số cột: {table.ColumnCount}\n\n";

        int namedCells = 0;
        var namedCellsList = new List<string>();

        for (int row = 0; row < table.RowCount; row++)
        {
            var tableRow = table.Rows[row];
            for (int col = 0; col < table.ColumnCount; col++)
            {
                try
                {
                    MyTableCell cell = tableRow[col];
                    if (cell != null && !string.IsNullOrEmpty(cell.Name))
                    {
                        namedCells++;
                        namedCellsList.Add($"  [{row},{col}] Name: '{cell.Name}', ID: {cell.Id}");
                    }
                }
                catch { }
            }
        }

        message += $"Số ô có Name: {namedCells}\n\n";

        if (namedCells > 0 && namedCells <= 20)
        {
            message += "=== CÁC Ô CÓ NAME ===\n";
            foreach (var item in namedCellsList)
            {
                message += item + "\n";
            }
        }
        else if (namedCells > 20)
        {
            message += "=== CÁC Ô CÓ NAME (hiển thị 20 ô đầu) ===\n";
            for (int i = 0; i < 20 && i < namedCellsList.Count; i++)
            {
                message += namedCellsList[i] + "\n";
            }
            message += $"  ... và {namedCells - 20} ô khác\n";
        }

        message += $"\n💡 Xem chi tiết trong Output Window của Visual Studio";
        message += $"\n💡 File CSV đã được export ra Desktop";

        TaskDialog.Show("ID Map Information", message);
    }

    /// <summary>
    /// Tìm tất cả các ô theo tên (Name)
    /// </summary>
    public static List<(int Row, int Col, MyTableCell Cell)> FindCellsByName(MyTable table, string name)
    {
        var result = new List<(int Row, int Col, MyTableCell Cell)>();

        if (table == null || string.IsNullOrEmpty(name))
            return result;

        for (int row = 0; row < table.RowCount; row++)
        {
            var tableRow = table.Rows[row];
            for (int col = 0; col < table.ColumnCount; col++)
            {
                try
                {
                    MyTableCell cell = tableRow[col];
                    if (cell != null && cell.Name == name)
                    {
                        result.Add((row, col, cell));
                    }
                }
                catch { }
            }
        }

        return result;
    }

    /// <summary>
    /// Lấy danh sách tất cả các ID trong bảng
    /// </summary>
    public static List<string> GetAllIds(MyTable table)
    {
        var ids = new List<string>();

        if (table == null)
            return ids;

        for (int row = 0; row < table.RowCount; row++)
        {
            var tableRow = table.Rows[row];
            for (int col = 0; col < table.ColumnCount; col++)
            {
                try
                {
                    MyTableCell cell = tableRow[col];
                    if (cell != null && !string.IsNullOrEmpty(cell.Id))
                    {
                        ids.Add(cell.Id);
                    }
                }
                catch { }
            }
        }

        return ids;
    }
}