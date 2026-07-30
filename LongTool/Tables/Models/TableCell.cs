using LongTool.Tables.Models.Styles;
using System;

namespace LongTool.Tables.Models;

/// <summary>
/// Đại diện cho một ô trong bảng.
/// Chỉ chứa dữ liệu, không xử lý hình học hoặc render.
/// </summary>
public sealed class TableCell
{
    #region Identification

    /// <summary>
    /// ID duy nhất của ô (tự động sinh).
    /// </summary>
    public string Id { get; internal set; }


    /// <summary>
    /// Tên tham chiếu của ô.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    #endregion



    #region Span

    /// <summary>
    /// Số cột mà cell chiếm.
    /// </summary>
    public int ColSpan { get; set; } = 1;


    /// <summary>
    /// Số hàng mà cell chiếm.
    /// </summary>
    public int RowSpan { get; set; } = 1;

    #endregion



    #region Content

    /// <summary>
    /// Nội dung hiển thị của cell.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    #endregion



    #region Style

    /// <summary>
    /// Kiểu hiển thị riêng của cell.
    /// Mỗi cell sở hữu một Style độc lập.
    /// </summary>
    public TableCellStyle Style { get; } =
        new TableCellStyle();


    /// <summary>
    /// Kiểm tra cell có Style hay không.
    /// Giữ lại để tương thích với các module cũ.
    /// </summary>
    public bool HasStyle =>
        Style != null;

    #endregion



    #region Cross Line

    /// <summary>
    /// Có vẽ đường gạch chéo trong ô không.
    /// </summary>
    public bool HasCrossLine { get; set; } = false;


    /// <summary>
    /// Hướng của đường gạch chéo.
    /// </summary>
    public CrossLineDirection CrossDirection { get; set; }
        = CrossLineDirection.TopLeftToBottomRight;

    #endregion



    #region Constructor

    public TableCell()
    {
        Id =
            Guid.NewGuid()
            .ToString("N")
            .Substring(0, 8);
    }


    public TableCell(string text) : this()
    {
        Text =
            text ?? string.Empty;
    }

    #endregion



    #region Validation

    public void Validate()
    {
        if (ColSpan <= 0)
        {
            throw new InvalidOperationException(
                "ColSpan phải lớn hơn 0.");
        }


        if (RowSpan <= 0)
        {
            throw new InvalidOperationException(
                "RowSpan phải lớn hơn 0.");
        }


        Style.Validate();
    }

    #endregion



    public override string ToString()
    {
        return
            $"Cell [{Id}] " +
            $"Name:'{Name}' " +
            $"Span({RowSpan},{ColSpan}) " +
            $"Text:\"{Text}\"";
    }
}



/// <summary>
/// Hướng của đường gạch chéo.
/// </summary>
public enum CrossLineDirection
{
    /// <summary>
    /// Từ trên trái xuống dưới phải (\)
    /// </summary>
    TopLeftToBottomRight,


    /// <summary>
    /// Từ trên phải xuống dưới trái (/)
    /// </summary>
    TopRightToBottomLeft,


    /// <summary>
    /// Từ dưới trái lên trên phải (/)
    /// </summary>
    BottomLeftToTopRight
}