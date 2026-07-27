using System;

namespace LongTool.Tables.Engine;

/// <summary>
/// Đại diện cho một đoạn đường viền của bảng.
/// </summary>
public sealed class TableBorderSegment : IEquatable<TableBorderSegment>
{
    #region Properties

    /// <summary>
    /// Điểm đầu.
    /// </summary>
    public TablePoint Start { get; }


    /// <summary>
    /// Điểm cuối.
    /// </summary>
    public TablePoint End { get; }


    /// <summary>
    /// Kiểu đường viền.
    /// </summary>
    public TableBorderStyle Style { get; }

    #endregion



    #region Constructor

    public TableBorderSegment(
        TablePoint start,
        TablePoint end,
        TableBorderStyle style)
    {
        Start = start;
        End = end;

        Style = style ??
            throw new ArgumentNullException(nameof(style));
    }

    #endregion



    #region Equality


    public bool Equals(TableBorderSegment other)
    {
        if (other == null)
            return false;


        // A -> B giống B -> A
        return
            (Start.Equals(other.Start) &&
             End.Equals(other.End))
            ||
            (Start.Equals(other.End) &&
             End.Equals(other.Start));
    }



    public override bool Equals(object obj)
    {
        return Equals(obj as TableBorderSegment);
    }



    public override int GetHashCode()
    {
        // Không phụ thuộc thứ tự Start / End
        return
            Start.GetHashCode()
            ^
            End.GetHashCode();
    }


    #endregion



    public override string ToString()
    {
        return $"{Start} -> {End}";
    }
}