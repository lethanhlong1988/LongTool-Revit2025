using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using LongTool.UI.Common.SelectionDialog;

namespace LongTool.UI.Common.SelectionDialog.Providers
{
    public class LegendSelectionProvider : ISelectionProvider
    {
        private readonly Document _doc;

        public string Title => "Select Legend";

        public string Label => "Legend:";

        public string Description =>
            "Hãy lựa chọn một Legend mẫu để làm template vẽ bảng Door Board.";

        public LegendSelectionProvider(Document doc)
        {
            _doc = doc;
        }

        public IEnumerable<SelectionDialogItem> GetItems()
        {
            return new FilteredElementCollector(_doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .Where(IsSelectableLegend)
                .OrderBy(v => v.Name)
                .Select(v => new SelectionDialogItem
                {
                    Id = v.Id.Value,
                    Name = v.Name,
                    Tag = v
                })
                .ToList();
        }

        public SelectionDialogItem GetSavedItem() => null;

        public void SaveSelectedItem(SelectionDialogItem item)
        {
        }

        // ==================================================
        // FILTER
        // ==================================================

        private static bool IsSelectableLegend(View view)
        {
            if (view == null)
                return false;

            if (!view.IsValidObject)
                return false;

            if (view.IsTemplate)
                return false;

            return view.ViewType == ViewType.Legend;
        }
    }
}
