using System;

namespace LongTool.Tables.Models.Styles;

/// <summary>
/// Định nghĩa kiểu hiển thị của một TableCell.
/// Chỉ chứa dữ liệu, không xử lý render.
/// </summary>
public sealed class TableCellStyle
{
    #region Border

    /// <summary>
    /// Kiểu hiển thị đường viền.
    /// </summary>
    public TableBorderStyle Borders { get; } =
        new TableBorderStyle();

    #endregion



    #region Text

    /// <summary>
    /// Kiểu hiển thị nội dung text.
    /// </summary>
    public TableTextStyle Text { get; } =
        new TableTextStyle();

    #endregion



    #region Alignment

    /// <summary>
    /// Căn ngang nội dung.
    /// </summary>
    public TableHorizontalAlignment HorizontalAlignment
    {
        get;
        set;
    }
    = TableHorizontalAlignment.Left;



    /// <summary>
    /// Căn dọc nội dung.
    /// </summary>
    public TableVerticalAlignment VerticalAlignment
    {
        get;
        set;
    }
    = TableVerticalAlignment.Middle;

    #endregion



    #region Padding

    public double PaddingLeft
    {
        get;
        set;
    }
    = 2.0;



    public double PaddingRight
    {
        get;
        set;
    }
    = 2.0;



    public double PaddingTop
    {
        get;
        set;
    }
    = 2.0;



    public double PaddingBottom
    {
        get;
        set;
    }
    = 2.0;

    #endregion



    #region Methods

    /// <summary>
    /// Tạo bản sao hoàn toàn độc lập.
    /// </summary>
    public TableCellStyle Clone()
    {
        var style =
            new TableCellStyle
            {
                HorizontalAlignment =
                    HorizontalAlignment,

                VerticalAlignment =
                    VerticalAlignment,

                PaddingLeft =
                    PaddingLeft,

                PaddingRight =
                    PaddingRight,

                PaddingTop =
                    PaddingTop,

                PaddingBottom =
                    PaddingBottom
            };


        style.Borders.CopyFrom(
            Borders);


        style.Text.CopyFrom(
            Text);


        return style;
    }



    /// <summary>
    /// Kiểm tra tính hợp lệ.
    /// </summary>
    public void Validate()
    {
        if (PaddingLeft < 0)
            throw new InvalidOperationException(
                "PaddingLeft không được nhỏ hơn 0.");


        if (PaddingRight < 0)
            throw new InvalidOperationException(
                "PaddingRight không được nhỏ hơn 0.");


        if (PaddingTop < 0)
            throw new InvalidOperationException(
                "PaddingTop không được nhỏ hơn 0.");


        if (PaddingBottom < 0)
            throw new InvalidOperationException(
                "PaddingBottom không được nhỏ hơn 0.");


        Borders.Validate();

        Text.Validate();
    }

    #endregion



    public override string ToString()
    {
        return
            $"H:{HorizontalAlignment}, " +
            $"V:{VerticalAlignment}, " +
            $"Padding({PaddingLeft}, {PaddingTop}, {PaddingRight}, {PaddingBottom})";
    }
}