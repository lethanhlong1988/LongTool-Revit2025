using System.Collections.Generic;

namespace LongTool.Tables.Geometry;

/// <summary>
/// Comparer để so sánh 2 TableLine không phụ thuộc thứ tự Start/End.
/// </summary>
public sealed class TableLineEqualityComparer : IEqualityComparer<TableLine>
{
    public bool Equals(TableLine x, TableLine y)
    {
        if (x == null || y == null) return false;

        // Sử dụng Equals của TableLine
        return x.Equals(y);
    }

    public int GetHashCode(TableLine obj)
    {
        if (obj == null) return 0;

        // Sử dụng GetHashCode của TableLine
        return obj.GetHashCode();
    }
}