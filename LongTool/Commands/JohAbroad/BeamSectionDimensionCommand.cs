using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.Services.DimStyle;
using LongTool.Services.BeamDimension;

namespace LongTool.Commands.JohAbroad
{
    [Transaction(TransactionMode.Manual)]
    public class BeamSectionDimensionCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            View view = doc.ActiveView;

            if (view == null)
            {
                message = "Không tìm thấy View hiện tại.";
                return Result.Failed;
            }

            // 1. Collect beams
            List<FamilyInstance> beams =
                new FilteredElementCollector(doc, view.Id)
                    .OfCategory(BuiltInCategory.OST_StructuralFraming)
                    .WhereElementIsNotElementType()
                    .OfType<FamilyInstance>()
                    .ToList();

            // 2. Filter + service
            var service = new BeamSectionDimensionService();
            List<FamilyInstance> targetBeams =
                service.FilterBeamsPerpendicular(beams, view);

            if (targetBeams.Count == 0)
            {
                TaskDialog.Show(
                    "LongTool - Beam Dimension",
                    "Không tìm thấy dầm vuông góc với mặt phẳng bản vẽ.");
                return Result.Succeeded;
            }

            // 3. Chọn dim style
            var dimStyleService = new DimStyleSelectorService();
            DimensionType dimStyle = dimStyleService.Select(doc);
            if (dimStyle == null)
                return Result.Cancelled;

            // 4. Tạo dim
            int successCount = 0;
            List<string> failedInfo = new List<string>();

            using (Transaction transaction =
                new Transaction(doc, "LongTool - Dimension Beam Sections"))
            {
                transaction.Start();

                foreach (FamilyInstance beam in targetBeams)
                {
                    try
                    {
                        if (service.CreateDimensions(doc, view, beam, dimStyle))
                            successCount++;
                        else
                            failedInfo.Add(
                                $"Beam ID {beam.Id.Value}: không tạo được dim.");
                    }
                    catch (Exception ex)
                    {
                        failedInfo.Add(
                            $"Beam ID {beam.Id.Value}: " +
                            $"{ex.GetType().Name} - {ex.Message}");
                    }
                }

                transaction.Commit();
            }

            // 5. Kết quả
            string msg =
                $"Dim Style: {dimStyle.Name}\n" +
                $"Dầm tìm được: {targetBeams.Count}\n" +
                $"Dầm đã Dim: {successCount}";

            if (failedInfo.Count > 0)
            {
                msg += "\n\nLỗi:\n" +
                       string.Join("\n", failedInfo.Take(10));

                if (failedInfo.Count > 10)
                {
                    msg += $"\n... và {failedInfo.Count - 10} dầm khác.";
                }
            }

            TaskDialog.Show("LongTool - Beam Dimension", msg);

            return Result.Succeeded;
        }
    }
}