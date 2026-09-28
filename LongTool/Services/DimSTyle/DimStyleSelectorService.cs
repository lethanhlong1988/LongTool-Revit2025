using Autodesk.Revit.DB;
using LongTool.UI.DimStyle;

namespace LongTool.Services.DimStyle
{
    public class DimStyleSelectorService
    {
        // ============================================================
        // HIỂN THỊ CỬA SỔ CHỌN DIM STYLE
        // ============================================================

        public DimensionType Select(Document doc)
        {
            if (doc == null)
                return null;

            DimStyleSelectionView window =
                new DimStyleSelectionView(doc);

            bool? result =
                window.ShowDialog();

            if (result != true)
                return null;

            return window.SelectedDimStyle;
        }
    }
}
