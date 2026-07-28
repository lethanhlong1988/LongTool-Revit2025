using System;

namespace LongTool.Tables.Models.Styles;

/// <summary>
/// Định nghĩa kiểu hiển thị của text trong TableCell.
/// Chỉ chứa dữ liệu, không xử lý render.
/// </summary>
public sealed class TableTextStyle :
    IEquatable<TableTextStyle>
{
    #region Appearance


    /// <summary>
    /// Chiều cao chữ.
    /// Đơn vị: Millimeter (mm).
    /// </summary>
    public double FontSize { get; set; }
        = 2.5;



    /// <summary>
    /// Chữ in đậm.
    /// </summary>
    public bool Bold { get; set; }
        = false;



    /// <summary>
    /// Chữ in nghiêng.
    /// </summary>
    public bool Italic { get; set; }
        = false;


    #endregion



    #region Color


    /// <summary>
    /// Màu chữ.
    /// </summary>
    public TableColor TextColor { get; set; }
        = TableColor.Black;


    #endregion



    #region Methods


    /// <summary>
    /// Sao chép style.
    /// </summary>
    public TableTextStyle Clone()
    {
        return new TableTextStyle()
        {
            FontSize = FontSize,
            Bold = Bold,
            Italic = Italic,
            TextColor = TextColor
        };
    }



    /// <summary>
    /// Kiểm tra dữ liệu.
    /// </summary>
    public void Validate()
    {
        if (FontSize <= 0)
        {
            throw new InvalidOperationException(
                "FontSize phải lớn hơn 0.");
        }
    }


    #endregion



    #region Equality


    public bool Equals(
        TableTextStyle other)
    {
        if (other == null)
            return false;


        return
            FontSize.Equals(other.FontSize) &&
            Bold == other.Bold &&
            Italic == other.Italic &&
            TextColor == other.TextColor;
    }



    public override bool Equals(
        object obj)
    {
        return Equals(
            obj as TableTextStyle);
    }



    public override int GetHashCode()
    {
        return HashCode.Combine(
            FontSize,
            Bold,
            Italic,
            TextColor);
    }


    #endregion



    public override string ToString()
    {
        return
            $"Size:{FontSize}, " +
            $"Bold:{Bold}, " +
            $"Italic:{Italic}, " +
            $"Color:{TextColor}";
    }

    public void CopyFrom(
        TableTextStyle other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        FontSize = other.FontSize;
        Bold = other.Bold;
        Italic = other.Italic;
        TextColor = other.TextColor;
    }
}