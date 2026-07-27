using LongTool.Tables.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LongTool.Tables.Engine;

/// <summary>
/// Quản lý danh sách các cột của bảng.
/// </summary>
public class TableColumnCollection : IEnumerable<TableColumn>
{
    private readonly List<TableColumn> _columns;

    /// <summary>
    /// Khởi tạo tập hợp cột.
    /// </summary>
    public TableColumnCollection()
    {
        _columns = new List<TableColumn>();
    }

    /// <summary>
    /// Thêm một cột mới.
    /// </summary>
    public TableColumn Add(double width)
    {
        TableColumn column = new TableColumn(_columns.Count, width);

        _columns.Add(column);

        return column;
    }

    /// <summary>
    /// Thêm một cột đã tạo sẵn.
    /// </summary>
    public void Add(TableColumn column)
    {
        column.Index = _columns.Count;

        _columns.Add(column);
    }

    /// <summary>
    /// Xóa toàn bộ cột.
    /// </summary>
    public void Clear()
    {
        _columns.Clear();
    }

    /// <summary>
    /// Số lượng cột.
    /// </summary>
    public int Count
    {
        get
        {
            return _columns.Count;
        }
    }

    /// <summary>
    /// Tổng chiều rộng của bảng.
    /// </summary>
    public double TotalWidth
    {
        get
        {
            return _columns.Sum(c => c.Width);
        }
    }

    /// <summary>
    /// Lấy tọa độ X của một cột.
    /// </summary>
    public double GetX(int columnIndex)
    {
        double x = 0;

        for (int i = 0; i < columnIndex; i++)
        {
            x += _columns[i].Width;
        }

        return x;
    }

    /// <summary>
    /// Truy cập cột theo chỉ số.
    /// </summary>
    public TableColumn this[int index]
    {
        get
        {
            return _columns[index];
        }
    }

    /// <summary>
    /// Enumerator.
    /// </summary>
    public IEnumerator<TableColumn> GetEnumerator()
    {
        return _columns.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}