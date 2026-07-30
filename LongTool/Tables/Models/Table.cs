using System;
using System.Collections.Generic;
using LongTool.Tables.Models.Styles;

namespace LongTool.Tables.Models;

/// <summary>
/// Đại diện cho một bảng dữ liệu.
/// Chỉ chứa dữ liệu và kiểm tra tính hợp lệ.
/// Không chứa logic render.
/// </summary>
public sealed class Table
{
    public TableDefaultStyle DefaultStyle { get; } =
    new TableDefaultStyle();

    private readonly List<TableRow> _rows;

    private readonly List<TableColumn> _columns;



    public IReadOnlyList<TableRow> Rows =>
        _rows;



    public IReadOnlyList<TableColumn> Columns =>
        _columns;



    public int RowCount =>
        _rows.Count;



    public int ColumnCount =>
        _columns.Count;




    public Table()
    {
        _rows = new List<TableRow>();

        _columns = new List<TableColumn>();
    }






    #region Column Management



    public void AddColumn(double width)
    {
        if (width <= 0)
            throw new ArgumentException(
                "Column width phải lớn hơn 0.",
                nameof(width));


        _columns.Add(
            new TableColumn(width));
    }





    public void ClearColumns()
    {
        _columns.Clear();
    }



    #endregion






    #region Row Management



    public TableRow AddRow(double height = 1)
    {
        if (height <= 0)
            throw new ArgumentException(
                "Row height phải lớn hơn 0.",
                nameof(height));


        var row =
            new TableRow(height);


        _rows.Add(row);


        return row;
    }





    public void ClearRows()
    {
        _rows.Clear();
    }



    #endregion






    #region Cell Search



    /// <summary>
    /// Tìm Cell theo Name.
    /// </summary>
    public TableCell GetCell(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;


        foreach (var row in _rows)
        {
            foreach (var cell in row)
            {
                if (cell.Name == name)
                {
                    return cell;
                }
            }
        }


        return null;
    }



    #endregion






    #region Validation



    /// <summary>
    /// Kiểm tra tính hợp lệ của bảng.
    /// </summary>
    public void Validate()
    {
        if (RowCount == 0)
            throw new InvalidOperationException(
                "Table không có Row.");


        if (ColumnCount == 0)
            throw new InvalidOperationException(
                "Table không có Column.");


        foreach (var row in _rows)
        {
            row.Validate(ColumnCount);
        }
    }



    #endregion






    public override string ToString()
    {
        return $"Table Rows:{RowCount}, Columns:{ColumnCount}";
    }
}