using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.RevitAdapters.Repositories;
using LongTool.UI.Common.FormDialog;
using LongTool.UI.Common.SelectionDialog;
using LongTool.UI.Common.SelectionDialog.Providers;
using System;
using System.Text;

namespace LongTool.Commands.Test;

[Transaction(TransactionMode.ReadOnly)]
public class TestLegendSelectionCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        try
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            var report = new StringBuilder();

            // ============================================================
            // TẦNG A: RevitLegendRepository — query trực tiếp
            // ============================================================

            report.AppendLine("=== TẦNG A: RevitLegendRepository ===");
            report.AppendLine();

            var repo = new RevitLegendRepository(doc);

            var hasAny = repo.HasAny();
            var all = repo.GetAll();

            report.AppendLine($"HasAny()         = {hasAny}");
            report.AppendLine($"GetAll().Count   = {all.Count}");
            report.AppendLine();

            if (all.Count > 0)
            {
                report.AppendLine("Danh sách Legend:");
                foreach (var v in all)
                {
                    report.AppendLine(
                        $"  • Id={v.Id.Value}, Name=\"{v.Name}\"");
                }
            }
            else
            {
                report.AppendLine("⚠ Không có Legend View nào trong file.");
                report.AppendLine("  → Cần tạo Legend View thủ công trước.");
            }

            report.AppendLine();

            // ============================================================
            // TẦNG B: Thử tạo LegendSelectionProvider + FormDialogView
            // ============================================================

            report.AppendLine("=== TẦNG B: LegendSelectionProvider ===");
            report.AppendLine();

            var provider = new LegendSelectionProvider(doc);

            report.AppendLine($"LegendSelectionProvider tạo OK.");
            report.AppendLine($"Provider type: {provider.GetType().FullName}");
            report.AppendLine();

            // ============================================================
            // HIỂN THỊ REPORT TRƯỚC KHI MỞ DIALOG
            // ============================================================

            TaskDialog.Show("Test Legend Selection — Report", report.ToString());

            // ============================================================
            // TẦNG C: Mở dialog thật để xem dropdown có rỗng không
            // ============================================================

            var fields = new (string Key, ISelectionProvider Provider)[]
            {
                ("Legend", provider)
            };

            var form = new FormDialogView("Chọn Legend (test)", fields);

            if (form.ShowDialog() == true)
            {
                var picked = form.Results["Legend"].Value as View;

                TaskDialog.Show(
                    "Test Result",
                    $"Bạn đã chọn:\n\n" +
                    $"Name = \"{picked?.Name ?? "(null)"}\"\n" +
                    $"Id   = {(picked != null ? picked.Id.Value.ToString() : "(null)")}");
            }
            else
            {
                TaskDialog.Show("Test Result", "Bạn đã hủy dialog.");
            }

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