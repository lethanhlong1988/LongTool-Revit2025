using System;
using Autodesk.Revit.DB;

namespace LongTool.Services.Dimensions
{
    /// <summary>
    /// Đọc thông số từ DimensionType để tính các giá trị layout
    /// (leg length, text size, text offset...).
    /// </summary>
    public static class DimensionStyleHelper
    {
        // Fallback khi không đọc được từ style (mm)
        public const double FALLBACK_LEG_LENGTH_MM = 2.5;

        // ============================================================
        // LEG LENGTH = Text Offset + Text Size
        // ============================================================

        /// <summary>
        /// Trả về leg length (internal units).
        /// Nếu không đọc được → trả fallback.
        /// </summary>
        public static double GetLegLength(
            DimensionType style,
            double fallbackMm = FALLBACK_LEG_LENGTH_MM)
        {
            double fallback = UnitUtils.ConvertToInternalUnits(
                fallbackMm, UnitTypeId.Millimeters);

            if (style == null)
                return fallback;

            double textOffset = GetTextOffset(style);
            double textSize = GetTextSize(style);

            double total = textOffset + textSize;
            return total > 1e-9 ? total : fallback;
        }

        // ============================================================
        // TEXT OFFSET
        // ============================================================

        public static double GetTextOffset(DimensionType style)
        {
            if (style == null) return 0;

            try
            {
                Parameter p = style.LookupParameter("Text Offset");
                if (p != null && p.HasValue)
                    return p.AsDouble();
            }
            catch { }

            return 0;
        }

        // ============================================================
        // TEXT SIZE
        // ============================================================

        public static double GetTextSize(DimensionType style)
        {
            if (style == null) return 0;

            try
            {
                Parameter p = style.get_Parameter(BuiltInParameter.TEXT_SIZE);
                if (p != null && p.HasValue)
                    return p.AsDouble();
            }
            catch { }

            return 0;
        }
    }
}