using System;

namespace LongTool.Tables.Geometry;

/// <summary>
/// Điểm trong TableEngine.
/// Đơn vị: Revit Internal Unit (Feet).
/// </summary>
public readonly struct TablePoint :
    IEquatable<TablePoint>
{
    #region Static

    /// <summary>
    /// Điểm gốc (0,0).
    /// </summary>
    public static readonly TablePoint Zero =
        new(0, 0);

    #endregion



    #region Properties

    public double X { get; }

    public double Y { get; }

    /// <summary>
    /// Kiểm tra có phải điểm gốc hay không.
    /// </summary>
    public bool IsZero =>
        Equals(Zero);

    #endregion



    #region Constructor

    public TablePoint(
        double x = 0,
        double y = 0)
    {
        X = x;
        Y = y;
    }

    #endregion



    #region Offset

    /// <summary>
    /// Dịch chuyển điểm.
    /// </summary>
    public TablePoint Offset(
        double x,
        double y)
    {
        return new TablePoint(
            X + x,
            Y + y);
    }



    /// <summary>
    /// Dịch chuyển theo một vector.
    /// </summary>
    public TablePoint Offset(
        TablePoint vector)
    {
        return this + vector;
    }



    public TablePoint OffsetX(
        double value)
    {
        return new TablePoint(
            X + value,
            Y);
    }



    public TablePoint OffsetY(
        double value)
    {
        return new TablePoint(
            X,
            Y + value);
    }

    #endregion



    #region Distance

    /// <summary>
    /// Khoảng cách tới điểm khác.
    /// </summary>
    public double DistanceTo(
        TablePoint other)
    {
        return Math.Sqrt(
            DistanceSquaredTo(other));
    }



    /// <summary>
    /// Bình phương khoảng cách.
    /// Hữu ích khi chỉ cần so sánh khoảng cách.
    /// </summary>
    public double DistanceSquaredTo(
        TablePoint other)
    {
        double dx = X - other.X;
        double dy = Y - other.Y;

        return dx * dx + dy * dy;
    }

    #endregion



    #region Equality

    public bool Equals(
        TablePoint other)
    {
        return
            X.Equals(other.X) &&
            Y.Equals(other.Y);
    }



    public override bool Equals(
        object obj)
    {
        return obj is TablePoint other &&
               Equals(other);
    }



    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }



    public static bool operator ==(
        TablePoint left,
        TablePoint right)
    {
        return left.Equals(right);
    }



    public static bool operator !=(
        TablePoint left,
        TablePoint right)
    {
        return !left.Equals(right);
    }

    #endregion



    #region Operators

    public static TablePoint operator +(
        TablePoint a,
        TablePoint b)
    {
        return new TablePoint(
            a.X + b.X,
            a.Y + b.Y);
    }



    public static TablePoint operator -(
        TablePoint a,
        TablePoint b)
    {
        return new TablePoint(
            a.X - b.X,
            a.Y - b.Y);
    }

    #endregion



    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}