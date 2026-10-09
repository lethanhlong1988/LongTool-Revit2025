using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;
using System.Text;

namespace LongTool.Commands.Test;

[Transaction(TransactionMode.ReadOnly)]
public class TestLegendProviderCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        try
        {
            var uiApp = commandData.Application;
            var doc = uiApp.ActiveUIDocument.Document;

            var report = new StringBuilder();
            report.AppendLine("=== KIỂM TRA LEGEND ===");
            report.AppendLine();

            // ------------------------------------------------------------
            // TẦNG 1: Query trực tiếp từ Document
            // ------------------------------------------------------------

            report.AppendLine("--- TẦNG 1: Query trực tiếp từ Document ---");

            var allLegends = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .Where(v => v.ViewType == ViewType.Legend)
                .ToList();

            report.AppendLine($"Tổng số Legend trong file: {allLegends.Count}");

            var legendsNotTemplate = allLegends
                .Where(v => !v.IsTemplate)
                .ToList();

            report.AppendLine($"Trong đó, không phải template: {legendsNotTemplate.Count}");
            report.AppendLine();

            report.AppendLine("Chi tiết từng Legend:");

            foreach (var v in allLegends)
            {
                report.AppendLine(
                    $"  • Id={v.Id.Value}, " +
                    $"Name=\"{v.Name}\", " +
                    $"IsTemplate={v.IsTemplate}, " +
                    $"IsValidObject={v.IsValidObject}");
            }

            report.AppendLine();

            // ------------------------------------------------------------
            // TẦNG 2: Kiểm tra FamilySymbol của Door (nếu cần)
            // ------------------------------------------------------------

            report.AppendLine("--- TẦNG 2: FamilySymbol của Door ---");

            var doorSymbols = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Doors)
                .Cast<FamilySymbol>()
                .ToList();

            report.AppendLine($"Tổng số Door FamilySymbol: {doorSymbols.Count}");

            foreach (var s in doorSymbols.Take(20))   // chỉ in 20 cái đầu
            {
                var jName = s.LookupParameter("J_Name")?.AsString() ?? "(rỗng)";
                var jNumber = s.LookupParameter("J_Number")?.AsString() ?? "(rỗng)";

                report.AppendLine(
                    $"  • Id={s.Id.Value}, " +
                    $"Family=\"{s.FamilyName}\", " +
                    $"Type=\"{s.Name}\", " +
                    $"J_Name={jName}, J_Number={jNumber}");
            }

            report.AppendLine();

            // ------------------------------------------------------------
            // HIỂN THỊ REPORT
            // ------------------------------------------------------------

            TaskDialog.Show("Test Legend Provider", report.ToString());

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.ToString();
            TaskDialog.Show("Test Error", ex.ToString());
            return Result.Failed;
        }
    }
}