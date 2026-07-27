
using LongTool.Tables.Models;
using System;

namespace LongTool.Tables.Engine;

/// <summary>
/// Kết quả bố trí hình học của một TableCell.
/// Được tạo bởi TableLayout.Build()
/// </summary>
public sealed class TableCellLayout
{
    #region Core

    public TableLayout Layout { get; }

    public TableCell Cell { get; }

    public int RowIndex { get; }

    public int ColumnIndex { get; }


    /// <summary>
    /// Số hàng mà cell chiếm.
    /// </summary>
    public int RowSpan { get; internal set; } = 1;


    /// <summary>
    /// Số cột mà cell chiếm.
    /// </summary>
    public int ColumnSpan { get; internal set; } = 1;


    public int EndRow =>
        RowIndex + RowSpan - 1;


    public int EndColumn =>
        ColumnIndex + ColumnSpan - 1;


    #endregion



    #region Geometry


    /// <summary>
    /// Góc trái dưới.
    /// </summary>
    public TablePoint Origin { get; internal set; }


    public double Width { get; internal set; }


    public double Height { get; internal set; }



    public double Left =>
        Origin.X;


    public double Bottom =>
        Origin.Y;


    public double Right =>
        Left + Width;


    public double Top =>
        Bottom + Height;



    public double CenterX =>
        (Left + Right) / 2.0;


    public double CenterY =>
        (Bottom + Top) / 2.0;


    #endregion



    #region Points


    public TablePoint BottomLeft =>
        new(Left, Bottom);


    public TablePoint BottomRight =>
        new(Right, Bottom);


    public TablePoint TopLeft =>
        new(Left, Top);


    public TablePoint TopRight =>
        new(Right, Top);


    public TablePoint Center =>
        new(CenterX, CenterY);



    #endregion



    #region Boundary


    public bool IsFirstRow =>
        RowIndex == 0;


    public bool IsLastRow =>
        EndRow == Layout.Table.RowCount - 1;



    public bool IsFirstColumn =>
        ColumnIndex == 0;


    public bool IsLastColumn =>
        EndColumn == Layout.Table.ColumnCount - 1;



    #endregion



    #region Constructor


    public TableCellLayout(
        TableLayout layout,
        TableCell cell,
        int rowIndex,
        int columnIndex)
    {
        Layout =
            layout ??
            throw new ArgumentNullException(nameof(layout));


        Cell =
            cell ??
            throw new ArgumentNullException(nameof(cell));


        RowIndex = rowIndex;
        ColumnIndex = columnIndex;

        Origin = new TablePoint();
    }


    #endregion



    public override string ToString()
    {
        return
            $"Cell [{RowIndex},{ColumnIndex}] " +
            $"Span({RowSpan},{ColumnSpan}) " +
            $"Size({Width}x{Height})";
    }


}