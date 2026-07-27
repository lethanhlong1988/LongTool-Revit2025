using System;
using System.Collections;
using System.Collections.Generic;

namespace LongTool.Tables.Models;

/// <summary>
/// Đại diện cho một hàng trong bảng.
/// Quản lý danh sách Cell và chiều cao hàng.
/// </summary>
public sealed class TableRow : IReadOnlyList<TableCell>
{
    private readonly List<TableCell> _cells;



    /// <summary>
    /// Chiều cao của hàng.
    /// </summary>
    public double Height { get; set; }



    /// <summary>
    /// Số Cell trong hàng.
    /// </summary>
    public int Count =>
        _cells.Count;



    public TableCell this[int index]
    {
        get
        {
            return _cells[index];
        }
    }



    public TableRow(
        double height = 1)
    {
        if (height <= 0)
            throw new ArgumentException(
                "Row height phải lớn hơn 0.",
                nameof(height));


        Height = height;

        _cells = new List<TableCell>();
    }





    #region Cell Management



    /// <summary>
    /// Thêm Cell mới vào cuối hàng.
    /// </summary>
    public TableCell AddCell()
    {
        var cell =
            new TableCell();


        _cells.Add(cell);


        return cell;
    }




    /// <summary>
    /// Thêm Cell đã tạo sẵn.
    /// </summary>
    public void AddCell(
        TableCell cell)
    {
        if (cell == null)
            throw new ArgumentNullException(
                nameof(cell));


        _cells.Add(cell);
    }





    /// <summary>
    /// Xóa toàn bộ Cell.
    /// </summary>
    public void Clear()
    {
        _cells.Clear();
    }





    /// <summary>
    /// Xóa Cell tại vị trí.
    /// </summary>
    public void RemoveAt(
        int index)
    {
        _cells.RemoveAt(index);
    }




    #endregion





    #region Validation



    /// <summary>
    /// Kiểm tra dữ liệu của Row.
    /// </summary>
    public void Validate(
        int columnCount)
    {
        if (columnCount <= 0)
            throw new ArgumentException(
                "columnCount không hợp lệ.");



        int usedColumns = 0;



        foreach (var cell in _cells)
        {
            if (cell.ColSpan <= 0)
            {
                throw new InvalidOperationException(
                    "ColSpan phải lớn hơn 0.");
            }


            if (cell.RowSpan <= 0)
            {
                throw new InvalidOperationException(
                    "RowSpan phải lớn hơn 0.");
            }



            usedColumns += cell.ColSpan;
        }



        if (usedColumns > columnCount)
        {
            throw new InvalidOperationException(
                $"Row vượt quá số Column. " +
                $"Đang dùng {usedColumns}/{columnCount}.");
        }
    }



    #endregion





    public IEnumerator<TableCell> GetEnumerator()
    {
        return _cells.GetEnumerator();
    }



    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }




    public override string ToString()
    {
        return
            $"Row Cells:{Count}, Height:{Height}";
    }


}

