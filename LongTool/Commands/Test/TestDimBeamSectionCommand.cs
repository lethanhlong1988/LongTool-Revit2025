using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using LongTool.Services.BeamDimension;
using LongTool.Services.DimStyle;

namespace LongTool.Commands.Test
{
    [Transaction(TransactionMode.Manual)]
    public class TestDimBeamSectionCommand : IExternalCommand
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

            // ------------------------------------------------------------
            // 1. CHỌN DẦM
            // ------------------------------------------------------------
            Reference pickedRef;
            try
            {
                pickedRef = uiDoc.Selection.PickObject(
                    ObjectType.Element,
                    new BeamSelectionFilter(),
                    "Chọn 1 dầm để test dim mặt cắt");
            }
            catch (OperationCanceledException)
            {
                return Result.Cancelled;
            }

            FamilyInstance beam = doc.GetElement(pickedRef) as FamilyInstance;
            if (beam == null)
            {
                TaskDialog.Show("Test Dim", "Element chọn không phải FamilyInstance.");
                return Result.Failed;
            }

            // ------------------------------------------------------------
            // 2. SERVICE + KIỂM TRA VUÔNG GÓC
            // ------------------------------------------------------------
            var service = new BeamSectionDimensionService();

            if (!service.IsPerpendicularToView(beam, view))
            {
                TaskDialog.Show("Test Dim",
                    "Dầm không vuông góc với mặt phẳng view hiện tại.");
                return Result.Failed;
            }

            // ------------------------------------------------------------
            // 3. CHỌN DIM STYLE
            // ------------------------------------------------------------
            var dimStyleService = new DimStyleSelectorService();
            DimensionType dimStyle = dimStyleService.Select(doc);

            if (dimStyle == null)
                return Result.Cancelled;

            // ------------------------------------------------------------
            // 4. TẠO DIM QUA SERVICE
            // ------------------------------------------------------------
            bool ok;
            using (Transaction tx = new Transaction(doc, "Test Dim Beam Section"))
            {
                tx.Start();
                ok = service.CreateDimensions(doc, view, beam, dimStyle);
                tx.Commit();
            }

            // ------------------------------------------------------------
            // 5. KẾT QUẢ
            // ------------------------------------------------------------
            TaskDialog.Show(
                "Test Dim Beam Section",
                $"Dầm ID: {beam.Id.Value}\n" +
                $"Dim Style: {dimStyle.Name}\n" +
                $"Tạo dim: {(ok ? "OK" : "FAIL")}");

            return Result.Succeeded;
        }

        // ============================================================
        // SELECTION FILTER
        // ============================================================

        private class BeamSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                return elem is FamilyInstance
                    && elem.Category != null
                    && elem.Category.Id.Value ==
                       (long)BuiltInCategory.OST_StructuralFraming;
            }

            public bool AllowReference(Reference reference, XYZ position) => false;
        }
    }
}