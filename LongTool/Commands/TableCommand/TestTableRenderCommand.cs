using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.Tables.Builders;
using LongTool.Tables.Data;
using LongTool.Tables.Diagnostics;
using LongTool.Tables.Layout;
using LongTool.Tables.Revit;
using LongTool.Tables.Renderers;
using LongTool.Tables.Rendering;
using LongTool.Tables.Services;
using LongTool.Tables.Models;
using LongTool.Core.Selection;
using System;
using System.Diagnostics;
using System.Linq;

namespace LongTool.Commands.TableCommand;

[Transaction(TransactionMode.Manual)]
public class TestTableRenderCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        try
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // 1. Pick Origin
            PickPointResult? pick =
                PointPicker.PickPoint(
                    uidoc,
                    "Chọn góc trái dưới của bảng");


            if (pick == null)
            {
                return Result.Cancelled;
            }


            XYZ origin =
                pick.Point;

            //--------------------------------------------------
            // 1. Tạo dữ liệu Door
            //--------------------------------------------------

            DoorData door = new DoorData
            {
                Mark = "D-001",
                Family = "Single Flush",
                Width = "900",
                Height = "2100",
                Material = "Steel"
            };

            //--------------------------------------------------
            // 2. Tạo Table
            //--------------------------------------------------

            Table table = DoorScheduleBuilder.CreateMergeTestTable();

            DoorScheduleService.Fill(table, door);

            TableDebugger.Show(table);

            using (Transaction trans =
                new Transaction(doc, "AutoFit Table"))
            {
                trans.Start();

                // đoạn AutoFit ở đây


            //--------------------------------------------------
            // Auto Fit Table
            //--------------------------------------------------

            TextNoteType textType =
                new FilteredElementCollector(doc)
                    .OfClass(typeof(TextNoteType))
                    .FirstElement() as TextNoteType;


            if (textType == null)
            {
                throw new InvalidOperationException(
                    "Không tìm thấy TextNoteType.");
            }


            ElementId textTypeId =
                textType.Id;

            TableTextMeasureService textMeasure =
                new TableTextMeasureService(
                    doc,
                    doc.ActiveView,
                    textTypeId);


            TableAutoFitService autoFit =
                new TableAutoFitService(
                    textMeasure);

            TableLayout tempLayout =
                new TableLayout(table);

            tempLayout.Build();

            autoFit.AutoFit(tempLayout);

            foreach (var column in table.Columns)
            {
                Debug.WriteLine(
                    $"AFTER AUTOFIT Column {column.Index}: {column.Width}");
            }

                trans.Commit();
            }

            //--------------------------------------------------
            // 3. Build Layout
            //--------------------------------------------------

            TableLayout layout = new TableLayout(table);

            layout.Build();

            LayoutDebugger.Show(
                layout.Cells);

            Debug.WriteLine("========== TABLE LAYOUT ==========");

            foreach (var cell in layout.Cells)
            {
                Debug.WriteLine(
                    $"{cell.Cell.Name} | " +
                    $"Text:{cell.Cell.Text} | " +
                    $"Row:{cell.RowIndex} Col:{cell.ColumnIndex} | " +
                   $"Left:{cell.Left} Bottom:{cell.Bottom} | " +
                    $"W:{cell.Width} H:{cell.Height}");
            }

            Debug.WriteLine("==================================");

            //--------------------------------------------------
            // 4. Render Debug
            //--------------------------------------------------

            DoorScheduleRenderer.Render(table);

            //--------------------------------------------------
            // 5. Render Revit
            //--------------------------------------------------

            RevitRenderContext context =
                new RevitRenderContext(
                    doc,
                    doc.ActiveView,
                    origin);

            using (Transaction trans =
                    new Transaction(doc, "Test Render Table"))
            {
                trans.Start();


                BorderRenderer renderer =
                    new BorderRenderer(context);

                renderer.Render(layout);

                //CellCenterDebugRenderer centerRenderer =
                //    new CellCenterDebugRenderer(
                //        context);


                //centerRenderer.Render(layout);



                TableTextRenderer textRenderer =
                    new TableTextRenderer(
                        doc,
                        doc.ActiveView,
                        context);

                textRenderer.Render(layout);



                trans.Commit();
            }

            //--------------------------------------------------
            // 6. Kết quả
            //--------------------------------------------------

            TaskDialog.Show(
                "Test Table",
                $"Cells : {layout.Cells.Count}\n" +
                $"Lines : {layout.BorderLines.Count}");

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.ToString();

            TaskDialog.Show(
                "Test Table Error",
                ex.ToString());

            return Result.Failed;
        }
    }
}