using Autodesk.Revit.DB;
using LongTool.Tables.Layout;
using LongTool.Tables.Models;
using System;
using System.Diagnostics;

namespace LongTool.Tables.Services;


/// <summary>
/// Tính toán kích thước Table dựa trên nội dung.
/// Không render.
/// </summary>
public sealed class TableAutoFitService
{
    private readonly TableTextMeasureService _textMeasureService;


    public TableAutoFitService(
        TableTextMeasureService textMeasureService)
    {
        _textMeasureService =
            textMeasureService ??
            throw new ArgumentNullException(
                nameof(textMeasureService));
    }

    public void AutoFit(
        TableLayout layout)
    {
        if (layout == null)
            throw new ArgumentNullException(nameof(layout));


        if (!layout.IsBuilt)
        {
            layout.Build();
        }


        Table table = layout.Table;



        foreach (var cellLayout in layout.Cells)
        {
            TableCell cell =
                cellLayout.Cell;


            if (string.IsNullOrEmpty(cell.Text))
                continue;



            int startColumn =
                cellLayout.ColumnIndex;


            int span =
                cellLayout.ColumnSpan;


            XYZ measurePoint =
                new XYZ(
                    cellLayout.TextAnchor.X,
                    cellLayout.TextAnchor.Y,
                    0);


            TextMeasureResult measureResult =
                _textMeasureService.Measure(
                    cell.Text,
                    measurePoint);


            double requiredWidth =
                measureResult.Width;



            double currentWidth = 0;


            for (int i = 0; i < span; i++)
            {
                currentWidth +=
                    table.Columns[startColumn + i].Width;
            }

            Debug.WriteLine(
                $"AutoFit {cell.Name}: " +
                $"Col={startColumn}, " +
                $"TextWidth={requiredWidth}, " +
                $"CurrentWidth={currentWidth}");

            if (requiredWidth > currentWidth)
            {
                double extra =
                    requiredWidth - currentWidth;


                int targetColumn =
                    startColumn + span - 1;


                table.Columns[targetColumn].Width += extra;
            }
        }
    }

}