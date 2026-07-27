using LongTool.Tables.Engine;  
using LongTool.Tables.Models;
using System;
using System.Collections.Generic;

namespace LongTool.Tables.Builders;

/// <summary>
/// Builder pattern để tạo bảng một cách dễ dàng.
/// </summary>
public class TableBuilder
{
    private Table _table;
    private TableRow _currentRow;
    private Dictionary<string, TableCell> _cellMap;

    public TableBuilder()
    {
        _table = new Table();
        _cellMap = new Dictionary<string, TableCell>();
    }

    #region Column Management

    public TableBuilder AddColumn(double width)
    {
        _table.AddColumn(width);
        return this;
    }

    public TableBuilder AddColumns(int count, double width)
    {
        for (int i = 0; i < count; i++)
        {
            _table.AddColumn(width);
        }
        return this;
    }

    public TableBuilder AddColumns(params double[] widths)
    {
        foreach (double width in widths)
        {
            _table.AddColumn(width);
        }
        return this;
    }

    #endregion

    #region Row Management

    public TableBuilder AddRow(double height = 10)
    {
        _currentRow = _table.AddRow(height);
        return this;
    }

    #endregion

    #region Cell Management

    public TableBuilder AddCell(string text, string name = null, Action<TableCell> configure = null)
    {
        if (_currentRow == null)
            throw new InvalidOperationException("Chưa có hàng nào. Gọi AddRow() trước.");

        TableCell cell = new TableCell(text);
        if (!string.IsNullOrEmpty(name))
        {
            cell.Name = name;
            _cellMap[name] = cell;
        }
        configure?.Invoke(cell);
        _currentRow.AddCell(cell);
        return this;
    }

    public TableBuilder AddMergedCell(string text, int colSpan = 1, int rowSpan = 1, string name = null, Action<TableCell> configure = null)
    {
        if (_currentRow == null)
            throw new InvalidOperationException("Chưa có hàng nào. Gọi AddRow() trước.");

        TableCell cell = new TableCell(text);
        cell.ColSpan = colSpan;
        cell.RowSpan = rowSpan;
        if (!string.IsNullOrEmpty(name))
        {
            cell.Name = name;
            _cellMap[name] = cell;
        }
        configure?.Invoke(cell);
        _currentRow.AddCell(cell);
        return this;
    }

    public TableBuilder AddCrossCell(string text, CrossLineDirection direction = CrossLineDirection.TopLeftToBottomRight, string name = null, Action<TableCell> configure = null)
    {
        if (_currentRow == null)
            throw new InvalidOperationException("Chưa có hàng nào. Gọi AddRow() trước.");

        TableCell cell = new TableCell(text);
        cell.HasCrossLine = true;
        cell.CrossDirection = direction;
        if (!string.IsNullOrEmpty(name))
        {
            cell.Name = name;
            _cellMap[name] = cell;
        }
        configure?.Invoke(cell);
        _currentRow.AddCell(cell);
        return this;
    }

    public TableBuilder AddHeader(string text, int colSpan = 1, int rowSpan = 1, string name = null)
    {
        return AddMergedCell(text, colSpan, rowSpan, name, cell =>
        {
            cell.Style.HorizontalAlignment = TableHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = TableVerticalAlignment.Middle;
            cell.Style.Text.Bold = true;
            cell.Style.Text.FontSize = 3.5;
            cell.Style.Borders.Top = true;
            cell.Style.Borders.Bottom = true;
            cell.Style.Borders.Left = true;
            cell.Style.Borders.Right = true;
        });
    }

    public TableBuilder AddData(string text, TableHorizontalAlignment align = TableHorizontalAlignment.Center, string name = null)
    {
        return AddCell(text, name, cell =>
        {
            cell.Style.HorizontalAlignment = align;
            cell.Style.VerticalAlignment = TableVerticalAlignment.Middle;
            cell.Style.Borders.Top = true;
            cell.Style.Borders.Bottom = true;
            cell.Style.Borders.Left = true;
            cell.Style.Borders.Right = true;
        });
    }

    #endregion

    #region Build

    public Table Build()
    {
        return _table;
    }

    public TableCell GetCell(string name)
    {
        if (_cellMap.TryGetValue(name, out TableCell cell))
            return cell;

        throw new KeyNotFoundException($"Không tìm thấy ô có tên '{name}'.");
    }

    public static implicit operator Table(TableBuilder builder)
    {
        return builder.Build();
    }

    #endregion
}