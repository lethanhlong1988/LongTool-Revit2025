using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using LongTool.UI.Common.SelectionDialog;

namespace LongTool.UI.Common.SelectionDialog.Providers
{
    public class DoorSelectionProvider : ISelectionProvider
    {
        private readonly Document _doc;

        public string Title => "Select Door Type";
        public string Label => "Door Type:";
        public string Description =>
            "Hãy lựa chọn một loại cửa (nhóm theo ký hiệu J_Name-J_Number).";

        public DoorSelectionProvider(Document doc)
        {
            _doc = doc;
        }

        public IEnumerable<SelectionDialogItem> GetItems()
        {
            // 1. Lấy toàn bộ Door instance trong model
            List<FamilyInstance> doors =
                new FilteredElementCollector(_doc)
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .WhereElementIsNotElementType()
                    .OfType<FamilyInstance>()
                    .Where(d => d.IsValidObject && d.Symbol != null)
                    .ToList();

            // 2. Build symbol cho từng cửa
            var doorWithSymbol = doors
                .Select(d => new
                {
                    Door = d,
                    Symbol = BuildSymbol(d)
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Symbol))
                .ToList();

            // 3. Group theo Symbol — mỗi Symbol chỉ lấy 1 đại diện
            List<SelectionDialogItem> items = doorWithSymbol
                .GroupBy(x => x.Symbol, System.StringComparer.OrdinalIgnoreCase)
                .Select(g =>
                {
                    // Đại diện: cửa đầu tiên trong nhóm
                    FamilyInstance representative = g.First().Door;

                    int count = g.Count();

                    return new SelectionDialogItem
                    {
                        // Id = Id của cửa đại diện
                        // (chỉ dùng để phân biệt, không dùng để lookup)
                        Id = representative.Id.Value,

                        // Tên hiển thị: Symbol — Type — (W×H) — ×Count
                        Name = BuildDisplayName(
                            representative,
                            g.Key,
                            count),

                        // Tag = FamilySymbol (loại cửa)
                        // → vì ta chọn theo TYPE, không phải instance
                        Tag = representative.Symbol
                    };
                })
                .OrderBy(x => x.Name)
                .ToList();

            return items;
        }

        public SelectionDialogItem GetSavedItem() => null;

        public void SaveSelectedItem(SelectionDialogItem item) { }

        // ==================================================
        // BUILD SYMBOL — ĐỒNG BỘ VỚI DoorScheduleCommand
        // ==================================================

        private static string BuildSymbol(FamilyInstance door)
        {
            string jName = GetTextParameter(door, "J_Name");
            if (string.IsNullOrWhiteSpace(jName))
                jName = GetTextParameter(door.Symbol, "J_Name");

            string jNumber = GetTextParameter(door, "J_Number");
            if (string.IsNullOrWhiteSpace(jNumber))
                jNumber = GetTextParameter(door.Symbol, "J_Number");

            bool hasName = !string.IsNullOrWhiteSpace(jName);
            bool hasNumber = !string.IsNullOrWhiteSpace(jNumber);

            if (hasName && hasNumber)
                return $"{jName}-{jNumber}";

            if (hasName) return jName;
            if (hasNumber) return jNumber;

            return string.Empty;
        }

        // ==================================================
        // BUILD DISPLAY NAME
        // ==================================================

        private static string BuildDisplayName(
            FamilyInstance door,
            string symbol,
            int count)
        {
            string typeName = door.Symbol?.Name ?? door.Name;

            double w = GetLengthParameterInMillimeters(door, "Width");
            if (w == 0)
                w = GetLengthParameterInMillimeters(door.Symbol, "Width");

            double h = GetLengthParameterInMillimeters(door, "Height");
            if (h == 0)
                h = GetLengthParameterInMillimeters(door.Symbol, "Height");

            string size = (w > 0 && h > 0)
                ? $"  ({w:0.#}×{h:0.#})"
                : string.Empty;

            string countStr = count > 1 ? $"  ×{count}" : string.Empty;

            return $"{symbol}  —  {typeName}{size}{countStr}";
        }

        // ==================================================
        // PARAMETER HELPERS
        // ==================================================

        private static string GetTextParameter(
            Element? element,
            string parameterName)
        {
            if (element == null) return string.Empty;

            Parameter? p = element.LookupParameter(parameterName);
            return p?.AsString() ?? string.Empty;
        }

        private static double GetLengthParameterInMillimeters(
            Element? element,
            string parameterName)
        {
            if (element == null) return 0;

            Parameter? p = element.LookupParameter(parameterName);

            if (p == null || p.StorageType != StorageType.Double)
                return 0;

            return UnitUtils.ConvertFromInternalUnits(
                p.AsDouble(),
                UnitTypeId.Millimeters);
        }
    }
}