using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using LongTool.UI.Common.SelectionDialog;

namespace LongTool.UI.Common.SelectionDialog.Providers
{
    /// <summary>
    /// Provider cho phép chọn một Legend Component (mặt đứng cửa).
    /// Chỉ liệt kê các FamilySymbol thuộc category OST_LegendComponents
    /// và có thể đặt được trong View Legend.
    ///
    /// Tương thích Revit 2025 & 2026:
    ///   - ElementId.IntegerValue đã bị remove.
    ///   - Chỉ dùng ElementId.Value (long).
    /// </summary>
    public class LegendComponentSelectionProvider : ISelectionProvider
    {
        private readonly Document _doc;

        public string Title => "Select Legend Component";

        public string Label => "Legend Component:";

        public string Description =>
            "Hãy lựa chọn một Legend Component (mặt đứng cửa) " +
            "để đặt vào ô 形状・寸法 của bảng Door Board.";

        public LegendComponentSelectionProvider(Document doc)
        {
            _doc = doc;
        }

        public IEnumerable<SelectionDialogItem> GetItems()
        {
            // 1. Lấy tất cả FamilySymbol thuộc category Legend Components
            List<FamilySymbol> symbols =
                new FilteredElementCollector(_doc)
                    .OfClass(typeof(FamilySymbol))
                    .Cast<FamilySymbol>()
                    .Where(IsLegendComponentSymbol)
                    .OrderBy(s => s.Family?.Name ?? string.Empty)
                    .ThenBy(s => s.Name)
                    .ToList();

            // 2. Build SelectionDialogItem cho từng symbol
            List<SelectionDialogItem> items = symbols
                .Select(s => new SelectionDialogItem
                {
                    // Id = Id của FamilySymbol (long, không dùng IntegerValue)
                    Id = s.Id.Value,

                    // Tên hiển thị: Family — Type
                    Name = BuildDisplayName(s),

                    // Tag = FamilySymbol (đối tượng thực sự cần lấy ra)
                    Tag = s
                })
                .ToList();

            return items;
        }

        public SelectionDialogItem GetSavedItem() => null;

        public void SaveSelectedItem(SelectionDialogItem item) { }

        // ==================================================
        // FILTER
        // ==================================================

        /// <summary>
        /// Kiểm tra một FamilySymbol có phải là Legend Component không.
        /// Điều kiện:
        ///  - Family có FamilyCategory khác null
        ///  - FamilyCategory.Id == OST_LegendComponents
        /// </summary>
        private static bool IsLegendComponentSymbol(FamilySymbol symbol)
        {
            if (symbol == null) return false;
            if (symbol.Family == null) return false;
            if (symbol.Family.FamilyCategory == null) return false;

            long categoryId =
                symbol.Family.FamilyCategory.Id.Value;

            return categoryId ==
                (long)BuiltInCategory.OST_LegendComponents;
        }

        // ==================================================
        // DISPLAY NAME
        // ==================================================

        private static string BuildDisplayName(FamilySymbol symbol)
        {
            string familyName =
                symbol.Family?.Name ?? "(no family)";

            string typeName =
                symbol.Name ?? "(no type)";

            // Nếu family name trùng type name thì chỉ hiển thị 1 lần
            if (string.Equals(
                    familyName,
                    typeName,
                    System.StringComparison.OrdinalIgnoreCase))
            {
                return familyName;
            }

            return $"{familyName}  —  {typeName}";
        }
    }
}