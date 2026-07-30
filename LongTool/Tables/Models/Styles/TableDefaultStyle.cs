using System;

namespace LongTool.Tables.Models.Styles;

/// <summary>
/// Style mặc định của Table.
/// Các Cell không có Style riêng sẽ sử dụng style này.
/// </summary>
public class TableDefaultStyle
{
    #region Cell Style

    /// <summary>
    /// Style mặc định áp dụng cho Cell.
    /// </summary>
    public TableCellStyle CellStyle { get; } =
        new TableCellStyle();

    #endregion



    #region Methods

    /// <summary>
    /// Kiểm tra dữ liệu hợp lệ.
    /// </summary>
    public void Validate()
    {
        CellStyle.Validate();
    }


    /// <summary>
    /// Tạo bản sao độc lập.
    /// </summary>
    public TableDefaultStyle Clone()
    {
        var style =
            new TableDefaultStyle();


        style.CellStyle.Borders.CopyFrom(
            CellStyle.Borders);


        style.CellStyle.Text.CopyFrom(
            CellStyle.Text);


        style.CellStyle.HorizontalAlignment =
            CellStyle.HorizontalAlignment;


        style.CellStyle.VerticalAlignment =
            CellStyle.VerticalAlignment;


        style.CellStyle.PaddingLeft =
            CellStyle.PaddingLeft;


        style.CellStyle.PaddingRight =
            CellStyle.PaddingRight;


        style.CellStyle.PaddingTop =
            CellStyle.PaddingTop;


        style.CellStyle.PaddingBottom =
            CellStyle.PaddingBottom;


        return style;
    }

    #endregion
}