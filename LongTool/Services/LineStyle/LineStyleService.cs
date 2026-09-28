using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace LongTool.Services.LineStyle
{
    public class LineStyleService
    {
        // ============================================================
        // LẤY CÁC LINE STYLE DÙNG CHO DETAIL LINE
        // ============================================================

        public List<GraphicsStyle> GetDetailLineStyles(Document doc)
        {
            Category linesCategory =
                doc.Settings.Categories.get_Item(
                    BuiltInCategory.OST_Lines);

            if (linesCategory == null)
            {
                return new List<GraphicsStyle>();
            }

            List<GraphicsStyle> result =
                new List<GraphicsStyle>();

            CategoryNameMap subCategories =
                linesCategory.SubCategories;

            foreach (Category subCategory in subCategories)
            {
                GraphicsStyle graphicsStyle =
                    subCategory.GetGraphicsStyle(
                        GraphicsStyleType.Projection);

                if (graphicsStyle != null)
                {
                    result.Add(graphicsStyle);
                }
            }

            return result
                .OrderBy(gs => gs.Name)
                .ToList();
        }
    }
}