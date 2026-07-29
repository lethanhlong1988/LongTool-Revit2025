using LongTool.Tables.Models;
using System;

namespace LongTool.Tables.Geometry;

/// <summary>
/// Xây dựng hình học của Table.
/// Chỉ chuyển dữ liệu Table thành Geometry.
/// Không thực hiện Render.
/// </summary>
public sealed class TableGeometryBuilder
{
    public TableGeometry Build(
        Table table)
    {
        ArgumentNullException.ThrowIfNull(table);

        table.Validate();

        var geometry =
            new TableGeometry();

        OccupancyGrid grid =
            CreateGrid(table);

        // Bước tiếp theo
        // CalculateCellRects(table, grid, geometry);

        return geometry;
    }

    /// <summary>
    /// Tạo OccupancyGrid từ Table.
    /// </summary>
    private OccupancyGrid CreateGrid(
        Table table)
    {
        var grid =
            new OccupancyGrid(
                table.RowCount,
                table.ColumnCount);

        for (int rowIndex = 0; rowIndex < table.RowCount; rowIndex++)
        {
            TableRow row =
                table.Rows[rowIndex];

            int columnIndex = 0;

            foreach (TableCell cell in row)
            {
                //
                // Tìm ô trống đầu tiên.
                //
                while (columnIndex < table.ColumnCount &&
                    grid.IsOccupied(rowIndex, columnIndex))
                {
                    columnIndex++;
                }
                grid.PlaceCell(
                    rowIndex,
                    columnIndex,
                    cell);

                columnIndex +=
                    cell.ColSpan;
            }
        }

        return grid;
    }
}