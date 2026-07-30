using System;

namespace LongTool.Tables.Models.Styles;

/// <summary>
/// Định nghĩa kiểu hiển thị đường viền của một ô.
/// </summary>
public sealed class TableBorderStyle :
    IEquatable<TableBorderStyle>
{
    #region Sides

    public bool Left { get; set; } = true;

    public bool Right { get; set; } = true;

    public bool Top { get; set; } = true;

    public bool Bottom { get; set; } = true;

    #endregion



    #region Appearance

    /// <summary>
    /// Chiều rộng đường viền.
    /// Đơn vị: mm.
    /// </summary>
    public double LineWidth { get; set; } = 1.0;

    #endregion



    #region Properties

    /// <summary>
    /// Không có cạnh nào được vẽ.
    /// </summary>
    public bool IsEmpty =>
        !Left &&
        !Right &&
        !Top &&
        !Bottom;

    #endregion



    #region Methods

    /// <summary>
    /// Sao chép từ BorderStyle khác.
    /// </summary>
    public void CopyFrom(
        TableBorderStyle other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        Left = other.Left;
        Right = other.Right;
        Top = other.Top;
        Bottom = other.Bottom;

        LineWidth = other.LineWidth;
    }



    /// <summary>
    /// Tạo bản sao.
    /// </summary>
    public TableBorderStyle Clone()
    {
        var style = new TableBorderStyle();

        style.CopyFrom(this);

        return style;
    }



    /// <summary>
    /// Kiểm tra dữ liệu.
    /// </summary>
    public void Validate()
    {
        if (LineWidth < 0)
        {
            throw new InvalidOperationException(
                "LineWidth không được nhỏ hơn 0.");
        }
    }

    #endregion



    #region Equality

    public bool Equals(
        TableBorderStyle other)
    {
        if (other == null)
            return false;

        return
            Left == other.Left &&
            Right == other.Right &&
            Top == other.Top &&
            Bottom == other.Bottom &&
            LineWidth.Equals(other.LineWidth);
    }



    public override bool Equals(
        object obj)
    {
        return Equals(
            obj as TableBorderStyle);
    }



    public override int GetHashCode()
    {
        return HashCode.Combine(
            Left,
            Right,
            Top,
            Bottom,
            LineWidth);
    }

    #endregion



    public override string ToString()
    {
        return
            $"L:{Left}, R:{Right}, T:{Top}, B:{Bottom}, Width:{LineWidth}";
    }
}