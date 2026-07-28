using System;

namespace LongTool.Tables.Models.Styles;

/// <summary>
/// Màu trong TableEngine.
/// Không phụ thuộc Revit.
/// </summary>
public readonly struct TableColor :
    IEquatable<TableColor>
{
    public byte R { get; }

    public byte G { get; }

    public byte B { get; }



    public TableColor(
        byte r,
        byte g,
        byte b)
    {
        R = r;
        G = g;
        B = b;
    }



    #region Preset Colors

    public static TableColor Black =>
        new(0, 0, 0);


    public static TableColor White =>
        new(255, 255, 255);


    public static TableColor Red =>
        new(255, 0, 0);


    public static TableColor Blue =>
        new(0, 0, 255);


    public static TableColor Green =>
        new(0, 255, 0);

    #endregion



    #region Equality

    public bool Equals(
        TableColor other)
    {
        return
            R == other.R &&
            G == other.G &&
            B == other.B;
    }



    public override bool Equals(
        object obj)
    {
        return obj is TableColor other &&
               Equals(other);
    }



    public override int GetHashCode()
    {
        return HashCode.Combine(
            R,
            G,
            B);
    }



    public static bool operator ==(
        TableColor left,
        TableColor right)
    {
        return left.Equals(right);
    }



    public static bool operator !=(
        TableColor left,
        TableColor right)
    {
        return !left.Equals(right);
    }

    #endregion



    public override string ToString()
    {
        return $"RGB({R},{G},{B})";
    }
}