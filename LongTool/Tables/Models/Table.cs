using LongTool.Tables.Engine;
using System;
using System.Collections.Generic;

namespace LongTool.Tables.Models;

/// <summary>
/// Đại diện cho một bảng dữ liệu.
/// Không chứa logic tính toán hình học.
/// </summary>
public sealed class Table
{
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



    public void AddColumn(
        double width)
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



    public TableRow AddRow(
        double height = 1)
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






    #region Layout



    /// <summary>
    /// Tạo layout từ cấu trúc hiện tại.
    /// </summary>
    public TableLayout CreateLayout()
    {
        Validate();


        var layout =
            new TableLayout(this);


        layout.Build();


        return layout;
    }



    #endregion






    #region Validation



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
            ValidateRow(row);
        }
    }





    private void ValidateRow(
        TableRow row)
    {
        int usedColumns = 0;



        foreach (var cell in row)
        {
            cell.Validate();


            usedColumns += cell.ColSpan;
        }



        if (usedColumns > ColumnCount)
        {
            throw new InvalidOperationException(
                $"Row vượt quá ColumnCount. " +
                $"Đang dùng {usedColumns}/{ColumnCount}.");
        }
    }



    #endregion





    public override string ToString()
    {
        return
            $"Table Rows:{RowCount}, Columns:{ColumnCount}";
    }
}
