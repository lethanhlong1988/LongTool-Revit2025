using Autodesk.Revit.DB;
using LongTool.UI.LineStyle;
using System.Windows;

namespace LongTool.Services.LineStyle
{
    public class LineStyleSelectorService
    {
        // ============================================================
        // HIỂN THỊ CỬA SỔ CHỌN LINE STYLE
        // ============================================================

        public GraphicsStyle Select(Document doc)
        {
            if (doc == null)
                return null;

            LineStyleSelectionView window =
                new LineStyleSelectionView(doc);

            bool? result =
                window.ShowDialog();

            if (result != true)
                return null;

            return window.SelectedLineStyle;
        }
    }
}