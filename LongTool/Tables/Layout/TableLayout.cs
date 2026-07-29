using LongTool.Tables.Geometry;
using LongTool.Tables.Models;
using System;
using System.Collections.Generic;

namespace LongTool.Tables.Layout;

/// <summary>
/// Tính toán vị trí hình học của toàn bộ bảng.
/// Hỗ trợ RowSpan và ColSpan.
/// </summary>
public sealed class TableLayout
{
    public Table Table { get; }

    public IReadOnlyList<TableCellLayout> Cells => _cells;
    public IReadOnlyList<TableLine> BorderLines => _borderLines;

    private readonly List<TableCellLayout> _cells;
    private readonly List<TableLine> _borderLines;

    private TableCellLayout[,] _matrix;
    private bool[,] _occupied;
    private double[] _columnX;
    private double[] _rowY;

    public bool IsBuilt { get; private set; }

    public TableLayout(Table table)
    {
        Table = table ?? throw new ArgumentNullException(nameof(table));
        _cells = new List<TableCellLayout>();
        _borderLines = new List<TableLine>();
        Initialize();
    }

    private void Initialize()
    {
        _matrix = new TableCellLayout[Table.RowCount, Table.ColumnCount];
        _occupied = new bool[Table.RowCount, Table.ColumnCount];
        _columnX = new double[Table.ColumnCount];
        _rowY = new double[Table.RowCount];
    }

    public void Build()
    {
        IsBuilt = false;

        Table.Validate();

        _cells.Clear();
        _borderLines.Clear();

        Initialize();

        BuildColumnCoordinates();
        BuildRowCoordinates();

        for (int r = 0; r < Table.RowCount; r++)
        {
            TableRow row = Table.Rows[r];
            foreach (TableCell cell in row)
            {
                BuildCell(cell);
            }
        }

        BuildBorderLines();

        IsBuilt = true;
    }

    private void BuildCell(TableCell cell)
    {
        cell.Validate();

        FindFreePosition(
            cell.RowSpan,
            cell.ColSpan,
            out int row,
            out int column);

        var layout = CreateCellLayout(cell, row, column);
        RegisterCell(layout);
    }

    private void FindFreePosition(
        int rowSpan,
        int columnSpan,
        out int row,
        out int column)
    {
        for (int r = 0; r < Table.RowCount; r++)
        {
            for (int c = 0; c < Table.ColumnCount; c++)
            {
                if (CanPlace(r, c, rowSpan, columnSpan))
                {
                    row = r;
                    column = c;
                    return;
                }
            }
        }

        throw new InvalidOperationException("Không tìm được vị trí cho Cell.");
    }

    private bool CanPlace(
        int row,
        int column,
        int rowSpan,
        int columnSpan)
    {
        if (row + rowSpan > Table.RowCount) return false;
        if (column + columnSpan > Table.ColumnCount) return false;

        for (int r = row; r < row + rowSpan; r++)
        {
            for (int c = column; c < column + columnSpan; c++)
            {
                if (_occupied[r, c]) return false;
            }
        }

        return true;
    }

    private TableCellLayout CreateCellLayout(
        TableCell cell,
        int row,
        int column)
    {
        var layout = new TableCellLayout(this, cell, row, column);

        layout.RowSpan = cell.RowSpan;
        layout.ColumnSpan = cell.ColSpan;

        layout.Origin = new TablePoint(_columnX[column], _rowY[row]);

        layout.Width = GetWidth(column, layout.ColumnSpan);
        layout.Height = GetHeight(row, layout.RowSpan);

        return layout;
    }

    private void RegisterCell(TableCellLayout layout)
    {
        _cells.Add(layout);

        for (int r = layout.RowIndex; r <= layout.EndRow; r++)
        {
            for (int c = layout.ColumnIndex; c <= layout.EndColumn; c++)
            {
                _occupied[r, c] = true;
                _matrix[r, c] = layout;
            }
        }
    }

    private void BuildColumnCoordinates()
    {
        double x = 0;

        for (int i = 0; i < Table.ColumnCount; i++)
        {
            _columnX[i] = x;
            x += Table.Columns[i].Width;
        }
    }

    private void BuildRowCoordinates()
    {
        double y = 0;

        for (int i = 0; i < Table.RowCount; i++)
        {
            _rowY[i] = y;
            y += Table.Rows[i].Height;
        }
    }

    private double GetWidth(int column, int span)
    {
        int last = column + span - 1;
        return _columnX[last] + Table.Columns[last].Width - _columnX[column];
    }

    private double GetHeight(int row, int span)
    {
        int last = row + span - 1;
        return _rowY[last] + Table.Rows[last].Height - _rowY[row];
    }

    #region Border Building

    private void BuildBorderLines()
    {
        _borderLines.Clear();

        var uniqueLines = new HashSet<TableLine>(new TableLineEqualityComparer());

        for (int r = 0; r < Table.RowCount; r++)
        {
            for (int c = 0; c < Table.ColumnCount; c++)
            {
                var cellLayout = _matrix[r, c];
                if (cellLayout == null) continue;

                var cell = cellLayout.Cell;
                var style = cell.Style.Borders;

                // Chỉ xử lý nếu đây là cell "chủ" (gốc) của merged area
                if (cellLayout.RowIndex == r && cellLayout.ColumnIndex == c)
                {
                    // TOP
                    if (style.Top && IsTopEdge(r, c))
                    {
                        AddBorderLine(uniqueLines,
                            cellLayout.TopLeft,
                            cellLayout.TopRight,
                            style.LineWidth);
                    }

                    // BOTTOM
                    if (style.Bottom && IsBottomEdge(r, c))
                    {
                        AddBorderLine(uniqueLines,
                            cellLayout.BottomLeft,
                            cellLayout.BottomRight,
                            style.LineWidth);
                    }

                    // LEFT
                    if (style.Left && IsLeftEdge(r, c))
                    {
                        AddBorderLine(uniqueLines,
                            cellLayout.TopLeft,
                            cellLayout.BottomLeft,
                            style.LineWidth);
                    }

                    // RIGHT
                    if (style.Right && IsRightEdge(r, c))
                    {
                        AddBorderLine(uniqueLines,
                            cellLayout.TopRight,
                            cellLayout.BottomRight,
                            style.LineWidth);
                    }
                }
            }
        }

        _borderLines.AddRange(uniqueLines);
    }

    // Helper methods để kiểm tra cạnh của merged area
    private bool IsTopEdge(int row, int col)
    {
        var current = _matrix[row, col];
        if (current == null) return false;

        if (current.RowIndex != row || current.ColumnIndex != col)
            return false;

        if (current.RowSpan > 1)
        {
            int startRow = current.RowIndex;
            return startRow == 0 || _matrix[startRow - 1, col] != current;
        }

        return row == 0 || _matrix[row - 1, col] != current;
    }

    private bool IsBottomEdge(int row, int col)
    {
        var current = _matrix[row, col];
        if (current == null) return false;

        if (current.RowIndex != row || current.ColumnIndex != col)
            return false;

        if (current.RowSpan > 1)
        {
            int endRow = current.RowIndex + current.RowSpan - 1;
            return endRow == Table.RowCount - 1 || _matrix[endRow + 1, col] != current;
        }

        return row == Table.RowCount - 1 || _matrix[row + 1, col] != current;
    }

    private bool IsLeftEdge(int row, int col)
    {
        var current = _matrix[row, col];
        if (current == null) return false;

        if (current.RowIndex != row || current.ColumnIndex != col)
            return false;

        if (current.ColumnSpan > 1)
        {
            int startColumn = current.ColumnIndex;
            return startColumn == 0 || _matrix[row, startColumn - 1] != current;
        }

        return col == 0 || _matrix[row, col - 1] != current;
    }

    private bool IsRightEdge(int row, int col)
    {
        var current = _matrix[row, col];
        if (current == null) return false;

        if (current.RowIndex != row || current.ColumnIndex != col)
            return false;

        if (current.ColumnSpan > 1)
        {
            int endColumn = current.ColumnIndex + current.ColumnSpan - 1;
            return endColumn == Table.ColumnCount - 1 || _matrix[row, endColumn + 1] != current;
        }

        return col == Table.ColumnCount - 1 || _matrix[row, col + 1] != current;
    }

    private void AddBorderLine(HashSet<TableLine> lines, TablePoint start, TablePoint end, double lineWidth)
    {
        var line = new TableLine(start, end, lineWidth);
        lines.Add(line);
    }

    #endregion

    public TableCellLayout this[int row, int column]
    {
        get
        {
            EnsureBuilt();
            return _matrix[row, column];
        }
    }

    public double GetColumnX(int column)
    {
        EnsureBuilt();
        return _columnX[column];
    }

    public double GetRowY(int row)
    {
        EnsureBuilt();
        return _rowY[row];
    }

    private void EnsureBuilt()
    {
        if (!IsBuilt)
        {
            throw new InvalidOperationException("TableLayout chưa được Build().");
        }
    }
}