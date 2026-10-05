using System;
using Autodesk.Revit.DB;
//using RevitDimension = Autodesk.Revit.DB.Dimension;

namespace LongTool.Services.Dimensions
{
    /// <summary>
    /// Tạo Dimension an toàn (có try/catch, kiểm tra reference trùng,
    /// kiểm tra dimension type hợp lệ).
    /// </summary>
    public static class DimensionFactory
    {
        // ============================================================
        // TẠO DIM TỪ 2 REFERENCE
        // ============================================================

        public static Dimension TryCreate(
            Document doc,
            View view,
            Reference ref1,
            Reference ref2,
            XYZ dimPosition,
            XYZ dimDirection,
            double halfLine,
            DimensionType dimStyle = null)
        {
            if (doc == null || view == null) return null;
            if (ref1 == null || ref2 == null) return null;

            // Kiểm tra 2 reference có trùng nhau không
            try
            {
                if (ref1.ElementId == ref2.ElementId &&
                    ref1.ConvertToStableRepresentation(doc) ==
                    ref2.ConvertToStableRepresentation(doc))
                {
                    return null;
                }
            }
            catch { }

            ReferenceArray refs = new ReferenceArray();
            refs.Append(ref1);
            refs.Append(ref2);

            Line dimLine = Line.CreateBound(
                dimPosition - dimDirection.Multiply(halfLine),
                dimPosition + dimDirection.Multiply(halfLine));

            try
            {
                Dimension dim = doc.Create.NewDimension(view, dimLine, refs);
                if (dim == null)
                    return null;

                ApplyStyle(dim, dimStyle);
                return dim;
            }
            catch
            {
                return null;
            }
        }

        // ============================================================
        // TẠO DIM TỪ 2 PLANARFACE
        // ============================================================

        public static Dimension TryCreate(
            Document doc,
            View view,
            PlanarFace face1,
            PlanarFace face2,
            XYZ dimPosition,
            XYZ dimDirection,
            double halfLine,
            DimensionType dimStyle = null,
            double normalTolerance = 0.90)
        {
            if (face1 == null || face2 == null) return null;
            if (face1.Reference == null || face2.Reference == null) return null;

            XYZ n1 = face1.FaceNormal.Normalize();
            XYZ n2 = face2.FaceNormal.Normalize();
            double dotNormals = Math.Abs(n1.DotProduct(n2));
            if (dotNormals < normalTolerance)
                return null;

            return TryCreate(
                doc, view,
                face1.Reference, face2.Reference,
                dimPosition, dimDirection, halfLine,
                dimStyle);
        }

        // ============================================================
        // KIỂM TRA DIMENSION TYPE CÓ PHẢI LINEAR
        // ============================================================

        public static bool IsLinear(DimensionType dt)
        {
            if (dt == null) return false;

            return dt.StyleType == DimensionStyleType.Linear
                || dt.StyleType == DimensionStyleType.LinearFixed;
        }

        // ============================================================
        // GÁN STYLE (NẾU HỢP LỆ)
        // ============================================================

        private static void ApplyStyle(Dimension dim, DimensionType style)
        {
            if (dim == null || style == null) return;
            if (dim.DimensionType == null) return;
            if (dim.DimensionType.Id == style.Id) return;
            if (!IsLinear(style)) return;

            try { dim.DimensionType = style; }
            catch { }
        }
    }
}