using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.UI.Common.FormDialog;
using LongTool.UI.Common.SelectionDialog.Providers;
using System;
using LongTool.UI.Common.SelectionDialog;

namespace LongTool.Commands.Test;

[Transaction(TransactionMode.Manual)]
public class TestFormDialogCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        UIDocument uidoc = commandData.Application.ActiveUIDocument;
        Document doc = uidoc.Document;

        try
        {
            // ============================================================
            // 1. KHAI BÁO CÁC FIELD (KEY + PROVIDER)
            // ============================================================

            var fields = new (string Key, ISelectionProvider Provider)[]
            {
                ("LineStyle", new LineStyleSelectionProvider(doc)),
                ("Door",      new DoorSelectionProvider(doc)),
                ("Legend",    new LegendSelectionProvider(doc))
            };


            // ============================================================
            // 2. MỞ FORM
            // ============================================================

            var form = new FormDialogView(
                "Cấu hình Tool Test",
                fields);


            if (form.ShowDialog() != true)
            {
                return Result.Cancelled;
            }


            // ============================================================
            // 3. LẤY KẾT QUẢ TỪNG FIELD
            // ============================================================

            GraphicsStyle lineStyle =
                form.Results["LineStyle"].Value as GraphicsStyle;

            FamilyInstance door =
                form.Results["Door"].Value as FamilyInstance;

            View legend =
                form.Results["Legend"].Value as View;


            // ============================================================
            // 4. HIỂN THỊ THÔNG BÁO
            // ============================================================

            TaskDialog.Show(
                "Kết quả Form",
                $"LineStyle: {lineStyle?.Name}\n" +
                $"Door:      {door?.Name}\n" +
                $"Legend:    {legend?.Name}");


            return Result.Succeeded;
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
        {
            return Result.Cancelled;
        }
        catch (Exception ex)
        {
            message = ex.ToString();
            return Result.Failed;
        }
    }
}