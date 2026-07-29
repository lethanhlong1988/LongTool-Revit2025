using LongTool.Tables.Models;
using System;

namespace LongTool.Tables.Geometry;

/// <summary>
/// Ma trận chiếm chỗ của Table.
/// Chỉ tồn tại trong quá trình Build Geometry.
/// </summary>
internal sealed class OccupancyGrid
{
    private readonly GridCell[,] _cells;

    public int RowCount { get; }

    public int ColumnCount { get; }

    public OccupancyGrid(
        int rowCount,
        int columnCount)
    {
        if (rowCount <= 0)
            throw new ArgumentException(
                "RowCount phải lớn hơn 0.",
                nameof(rowCount));

        if (columnCount <= 0)
            throw new ArgumentException(
                "ColumnCount phải lớn hơn 0.",
                nameof(columnCount));

        RowCount = rowCount;
        ColumnCount = columnCount;

        _cells = new GridCell[rowCount, columnCount];

        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                _cells[row, column] = new GridCell();
            }
        }
    }

    /// <summary>
    /// Đặt Cell vào Grid.
    /// Tự xử lý RowSpan, ColSpan và Origin.
    /// </summary>
    public void PlaceCell(
        int startRow,
        int startColumn,
        TableCell cell)
    {
        ArgumentNullException.ThrowIfNull(cell);

        ValidateIndex(startRow, startColumn);

        int endRow =
            startRow + cell.RowSpan - 1;

        int endColumn =
            startColumn + cell.ColSpan - 1;

        if (endRow >= RowCount)
        {
            throw new InvalidOperationException(
                "RowSpan vượt khỏi giới hạn bảng.");
        }

        if (endColumn >= ColumnCount)
        {
            throw new InvalidOperationException(
                "ColSpan vượt khỏi giới hạn bảng.");
        }

        // Kiểm tra trước
        for (int row = startRow; row <= endRow; row++)
        {
            for (int column = startColumn; column <= endColumn; column++)
            {
                if (_cells[row, column].IsOccupied)
                {
                    throw new InvalidOperationException(
                        $"Grid[{row},{column}] đã được sử dụng.");
                }
            }
        }

        // Gán Cell
        for (int row = startRow; row <= endRow; row++)
        {
            for (int column = startColumn; column <= endColumn; column++)
            {
                bool isOrigin =
                    row == startRow &&
                    column == startColumn;

                _cells[row, column].SetCell(
                    cell,
                    isOrigin);
            }
        }
    }

    public bool IsOccupied(
        int row,
        int column)
    {
        ValidateIndex(row, column);

        return _cells[row, column].IsOccupied;
    }

    public TableCell? GetCell(
        int row,
        int column)
    {
        ValidateIndex(row, column);

        return _cells[row, column].Cell;
    }

    public bool IsOrigin(
        int row,
        int column)
    {
        ValidateIndex(row, column);

        return _cells[row, column].IsOrigin;
    }

    public void Clear()
    {
        for (int row = 0; row < RowCount; row++)
        {
            for (int column = 0; column < ColumnCount; column++)
            {
                _cells[row, column].Clear();
            }
        }
    }

    private void ValidateIndex(
        int row,
        int column)
    {
        if (row < 0 || row >= RowCount)
            throw new ArgumentOutOfRangeException(nameof(row));

        if (column < 0 || column >= ColumnCount)
            throw new ArgumentOutOfRangeException(nameof(column));
    }

    public override string ToString()
    {
        return $"Grid {RowCount} x {ColumnCount}";
    }
}