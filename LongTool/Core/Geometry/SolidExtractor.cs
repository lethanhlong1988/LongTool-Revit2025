using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace LongTool.Core.Geometry
{
    /// <summary>
    /// Trích xuất Solid từ Element / GeometryElement.
    /// </summary>
    public static class SolidExtractor
    {
        // Ngưỡng thể tích mặc định để loại solid rác (internal units)
        public const double DEFAULT_MIN_VOLUME = 1e-6;

        // ============================================================
        // LẤY TẤT CẢ SOLID TỪ GEOMETRY ELEMENT
        // ============================================================

        public static List<Solid> GetAllSolids(
            GeometryElement geometry,
            double minVolume = DEFAULT_MIN_VOLUME)
        {
            List<Solid> result = new List<Solid>();

            if (geometry == null) return result;

            foreach (GeometryObject obj in geometry)
            {
                Solid solid = obj as Solid;
                if (solid == null) continue;
                if (solid.Volume <= minVolume) continue;

                result.Add(solid);
            }

            return result;
        }

        // ============================================================
        // LẤY TẤT CẢ SOLID TỪ ELEMENT
        // ============================================================

        public static List<Solid> GetAllSolids(
            Element element,
            double minVolume = DEFAULT_MIN_VOLUME,
            bool includeNonVisible = false,
            ViewDetailLevel detailLevel = ViewDetailLevel.Fine)
        {
            if (element == null) return new List<Solid>();

            Options opt = new Options
            {
                ComputeReferences = true,
                IncludeNonVisibleObjects = includeNonVisible,
                DetailLevel = detailLevel
            };

            GeometryElement geo = element.get_Geometry(opt);
            return GetAllSolids(geo, minVolume);
        }

        // ============================================================
        // LẤY SOLID LỚN NHẤT
        // ============================================================

        public static Solid GetLargestSolid(
            Element element,
            double minVolume = DEFAULT_MIN_VOLUME,
            bool includeNonVisible = false,
            ViewDetailLevel detailLevel = ViewDetailLevel.Fine)
        {
            var solids = GetAllSolids(
                element, minVolume, includeNonVisible, detailLevel);

            if (solids.Count == 0) return null;

            return solids.OrderByDescending(s => s.Volume).First();
        }

        // ============================================================
        // LẤY DANH SÁCH SOLID ĐÃ SẮP XẾP THEO THỂ TÍCH GIẢM DẦN
        // ============================================================

        public static List<Solid> GetAllSolidsSortedByVolume(
            Element element,
            double minVolume = DEFAULT_MIN_VOLUME,
            bool includeNonVisible = false,
            ViewDetailLevel detailLevel = ViewDetailLevel.Fine)
        {
            var solids = GetAllSolids(
                element, minVolume, includeNonVisible, detailLevel);

            return solids.OrderByDescending(s => s.Volume).ToList();
        }
    }
}