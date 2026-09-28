using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.UI.LineStyle;
using System.Windows;

namespace LongTool.Commands.JohAbroad
{
    [Transaction(TransactionMode.Manual)]
    public class TestLineStyleSelectionCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uidoc =
                commandData.Application.ActiveUIDocument;

            Document doc =
                uidoc.Document;

            LineStyleSelectionView window =
                new LineStyleSelectionView(doc);

            bool? result =
                window.ShowDialog();

            if (result == true &&
                window.SelectedLineStyle != null)
            {
                TaskDialog.Show(
                    "Selected Line Style",
                    $"Đã chọn: {window.SelectedLineStyle.Name}\n\n" +
                    $"ElementId: {window.SelectedLineStyle.Id}");
            }

            return Result.Succeeded;
        }
    }
}