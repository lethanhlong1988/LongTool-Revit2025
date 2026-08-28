using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;

namespace LongTool.Commands.JohAbroad
{
    [Transaction(TransactionMode.Manual)]
    public class HideLevelsCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            View view = doc.ActiveView;

            // Chỉ cho phép chạy trong Elevation hoặc Section
            if (view.ViewType != ViewType.Elevation &&
                view.ViewType != ViewType.Section)
            {
                TaskDialog.Show(
                    "Hide Levels",
                    "Lệnh này chỉ sử dụng trong Elevation hoặc Section View.");

                return Result.Cancelled;
            }

            // Lấy các Level trong model
            var levelIds = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .WhereElementIsNotElementType()
                .Select(e => e.Id)
                .ToList();

            if (levelIds.Count == 0)
            {
                TaskDialog.Show(
                    "Hide Levels",
                    "Không tìm thấy Level nào trong model.");

                return Result.Succeeded;
            }

            using (Transaction trans =
                   new Transaction(doc, "Hide Levels"))
            {
                trans.Start();

                try
                {
                    view.HideElements(levelIds);

                    trans.Commit();
                }
                catch (System.Exception ex)
                {
                    trans.RollBack();

                    TaskDialog.Show(
                        "Hide Levels",
                        "Không thể Hide Level.\n\n" + ex.Message);

                    return Result.Failed;
                }
            }

            return Result.Succeeded;
        }
    }
}