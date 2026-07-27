using LongTool.Tables.Engine;
using LongTool.Tables.Models;
using System;
using System.Collections.Generic;

namespace LongTool.Tables.Builders;

/// <summary>
/// Quản lý tham chiếu đến các ô trong bảng.
/// </summary>
public class TableReference
{
    private readonly Table _table;
    private readonly Dictionary<string, (int row, int col)> _cellPositions;
    private readonly Dictionary<string, TableCell> _cellReferences;

    public TableReference(Table table)
    {
        _table = table;
        _cellPositions = new Dictionary<string, (int, int)>();
        _cellReferences = new Dictionary<string, TableCell>();
    }

    /// <summary>
    /// Đăng ký một ô với tên tham chiếu.
    /// </summary>
    public void RegisterCell(string name, int row, int col)
    {
        if (row < 0 || row >= _table.RowCount)
            throw new ArgumentOutOfRangeException(nameof(row));

        if (col < 0 || col >= _table.ColumnCount)
            throw new ArgumentOutOfRangeException(nameof(col));

        var cell = _table.Rows[row][col];
        _cellPositions[name] = (row, col);
        _cellReferences[name] = cell;
    }

    /// <summary>
    /// Lấy ô theo tên.
    /// </summary>
    public TableCell GetCell(string name)
    {
        if (_cellReferences.TryGetValue(name, out TableCell cell))
            return cell;

        throw new KeyNotFoundException($"Không tìm thấy ô '{name}'.");
    }

    /// <summary>
    /// Lấy vị trí của ô theo tên.
    /// </summary>
    public (int row, int col) GetPosition(string name)
    {
        if (_cellPositions.TryGetValue(name, out var pos))
            return pos;

        throw new KeyNotFoundException($"Không tìm thấy ô '{name}'.");
    }

    /// <summary>
    /// Cập nhật text của ô theo tên.
    /// </summary>
    public void UpdateText(string name, string text)
    {
        var cell = GetCell(name);
        cell.Text = text;
    }

    /// <summary>
    /// Cập nhật style của ô theo tên.
    /// </summary>
    public void UpdateStyle(string name, Action<TableCellStyle> configure)
    {
        var cell = GetCell(name);
        configure?.Invoke(cell.Style);
    }

    /// <summary>
    /// In danh sách tất cả các ô đã đăng ký.
    /// </summary>
    public void PrintAllReferences()
    {
        Console.WriteLine("=== TABLE REFERENCES ===");
        foreach (var kvp in _cellPositions)
        {
            Console.WriteLine($"  {kvp.Key} -> ({kvp.Value.row}, {kvp.Value.col})");
        }
    }
}