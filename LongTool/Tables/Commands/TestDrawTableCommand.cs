using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.Infrastructure;
using LongTool.Renderers.Table;
using LongTool.UI.Table;
using LongTool.Utils;
using System;
using System.Linq;

namespace LongTool.Tables.Commands;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class TestDrawTableCommand : CommandBase
{
    public override Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        TableNameWindow window = new TableNameWindow();
        bool? result = window.ShowDialog();

        if (result != true)
        {
            return Result.Cancelled;
        }

        Document document = commandData.Application
                                       .ActiveUIDocument
                                       .Document;

        // Kiểm tra tên view đã tồn tại TRƯỚC khi tạo
        string viewName = window.TableName.Trim();
        if (string.IsNullOrEmpty(viewName))
        {
            TaskDialog.Show("LongTool", "Table name cannot be empty.");
            return Result.Cancelled;
        }

        // Kiểm tra view đã tồn tại
        bool viewExists = new FilteredElementCollector(document)
            .OfClass(typeof(ViewDrafting))
            .Cast<ViewDrafting>()
            .Any(v => v.Name.Equals(viewName, System.StringComparison.OrdinalIgnoreCase));

        if (viewExists)
        {
            TaskDialog.Show("LongTool", $"A view with the name \"{viewName}\" already exists.");
            return Result.Cancelled;
        }

        // Lấy ViewFamilyType
        ViewFamilyType draftingViewType = new FilteredElementCollector(document)
            .OfClass(typeof(ViewFamilyType))
            .Cast<ViewFamilyType>()
            .FirstOrDefault(x => x.ViewFamily == ViewFamily.Drafting);

        if (draftingViewType == null)
        {
            TaskDialog.Show("LongTool", "No Drafting View Family Type found.");
            return Result.Cancelled;
        }

        ViewDrafting draftingView = null;

        // Transaction 1: Tạo Drafting View
        using (Transaction transaction = new Transaction(document, "Create Drafting View"))
        {
            try
            {
                transaction.Start();

                // Tạo Drafting View
                draftingView = ViewDrafting.Create(
                    document,
                    draftingViewType.Id);

                // Set tên
                draftingView.Name = viewName;

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.RollBack();
                TaskDialog.Show("LongTool", $"Error creating view: {ex.Message}");
                return Result.Failed;
            }
        }

        // Chuyển sang Drafting View vừa tạo
        UIDocument uiDocument = commandData.Application.ActiveUIDocument;
        uiDocument.RequestViewChange(draftingView);

        // Transaction 2: Vẽ hình vuông với đơn vị mm
        using (Transaction drawTransaction = new Transaction(document, "Draw Square"))
        {
            try
            {
                drawTransaction.Start();

                TableRenderer renderer = new TableRenderer(document, draftingView);

                // Vẽ hình vuông với kích thước 500mm
                double sizeInMm = 500;
                renderer.DrawSquare(sizeInMm);

                drawTransaction.Commit();

                ShowMessage(
                    "LongTool",
                    $"Drafting View \"{viewName}\" created successfully with square (500mm).");

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                drawTransaction.RollBack();
                TaskDialog.Show("LongTool", $"Error drawing square: {ex.Message}");
                return Result.Failed;
            }
        }
    }
}