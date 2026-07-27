using System;

namespace LongTool.Tables.Engine;

/// <summary>
/// Biểu diễn một đoạn đường viền của bảng.
/// </summary>
public sealed class TableLine : IEquatable<TableLine>
{
    #region Properties

    public TablePoint Start { get; }

    public TablePoint End { get; }

    /// <summary>
    /// Chiều rộng đường viền.
    /// Đơn vị: Feet (Revit Internal Unit).
    /// </summary>
    public double LineWidth { get; }

    public double Length => Start.DistanceTo(End);

    #endregion

    #region Constructor

    public TableLine(TablePoint start, TablePoint end, double lineWidth = 1.0)
    {
        Start = start;
        End = end;
        LineWidth = lineWidth;
    }

    #endregion

    #region Equality

    /// <summary>
    /// So sánh 2 đoạn thẳng không phụ thuộc thứ tự Start/End.
    /// </summary>
    public bool Equals(TableLine other)
    {
        if (other == null) return false;

        // A->B giống B->A
        return (Start.Equals(other.Start) && End.Equals(other.End)) ||
               (Start.Equals(other.End) && End.Equals(other.Start));
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as TableLine);
    }

    public override int GetHashCode()
    {
        // XOR để không phụ thuộc thứ tự Start/End
        return Start.GetHashCode() ^ End.GetHashCode();
    }

    #endregion

    #region Overrides

    public override string ToString()
    {
        return $"{Start} -> {End} (Width: {LineWidth})";
    }

    #endregion
}