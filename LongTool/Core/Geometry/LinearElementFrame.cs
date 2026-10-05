using System;
using Autodesk.Revit.DB;

namespace LongTool.Core.Geometry
{
    /// <summary>
    /// Hệ trục local của một phần tử tuyến tính (beam / column / wall)
    /// trong không gian của một View:
    ///   Axis      - dọc theo phần tử, hướng cùng chiều View.ViewDirection
    ///   WidthDir  - ngang (projection của View.RightDirection lên mặt phẳng vuông Axis)
    ///   HeightDir - Axis × WidthDir, hướng cùng chiều View.UpDirection
    /// </summary>
    public class LinearElementFrame
    {
        public XYZ Axis { get; }
        public XYZ WidthDir { get; }
        public XYZ HeightDir { get; }

        private LinearElementFrame(XYZ axis, XYZ widthDir, XYZ heightDir)
        {
            Axis = axis;
            WidthDir = widthDir;
            HeightDir = heightDir;
        }

        // ============================================================
        // TẠO TỪ FAMILY INSTANCE (BEAM / COLUMN)
        // ============================================================

        /// <summary>
        /// Trả về null nếu element không có LocationCurve hợp lệ.
        /// </summary>
        public static LinearElementFrame FromInstance(
            FamilyInstance instance,
            View view)
        {
            if (instance == null || view == null)
                return null;

            LocationCurve lc = instance.Location as LocationCurve;
            if (lc == null)
                return null;

            XYZ p0 = lc.Curve.GetEndPoint(0);
            XYZ p1 = lc.Curve.GetEndPoint(1);

            XYZ dir = p1 - p0;
            if (dir.IsZeroLength())
                return null;

            dir = dir.Normalize();

            return Create(dir, view);
        }

        // ============================================================
        // TẠO TỪ AXIS + VIEW
        // ============================================================

        /// <summary>
        /// axis: vector đơn vị dọc phần tử (chưa cần cùng chiều view).
        /// Trả về null nếu axis không hợp lệ.
        /// </summary>
        public static LinearElementFrame Create(XYZ axis, View view)
        {
            if (axis == null || view == null)
                return null;
            if (axis.IsZeroLength())
                return null;

            axis = axis.Normalize();

            // Đảm bảo axis hướng cùng chiều ViewDirection
            if (axis.DotProduct(view.ViewDirection) < 0)
                axis = axis.Negate();

            XYZ widthDir = ComputeWidthDirection(axis, view);
            if (widthDir == null)
                return null;

            XYZ heightDir = axis.CrossProduct(widthDir).Normalize();

            // Đảm bảo heightDir cùng chiều View.UpDirection
            if (heightDir.DotProduct(view.UpDirection) < 0)
                heightDir = heightDir.Negate();

            return new LinearElementFrame(axis, widthDir, heightDir);
        }

        // ============================================================
        // WIDTH DIRECTION (PROJECTION VIEW.RIGHT LÊN MẶT PHẲNG VUÔNG AXIS)
        // ============================================================

        private static XYZ ComputeWidthDirection(XYZ axis, View view)
        {
            // Ưu tiên 1: chiếu View.RightDirection
            XYZ projected = ProjectOntoPlane(view.RightDirection, axis);
            if (projected != null) return projected;

            // Ưu tiên 2: chiếu View.UpDirection
            projected = ProjectOntoPlane(view.UpDirection, axis);
            if (projected != null) return projected;

            // Ưu tiên 3: chiếu BasisX
            projected = ProjectOntoPlane(XYZ.BasisX, axis);
            if (projected != null) return projected;

            // Fallback: vector vuông góc bất kỳ
            return GetPerpendicular(axis);
        }

        private static XYZ ProjectOntoPlane(XYZ v, XYZ planeNormal)
        {
            if (v == null || planeNormal == null) return null;

            XYZ projected = v - planeNormal.Multiply(v.DotProduct(planeNormal));
            return projected.IsZeroLength() ? null : projected.Normalize();
        }

        // ============================================================
        // LẤY VECTOR VUÔNG GÓC BẤT KỲ
        // ============================================================

        public static XYZ GetPerpendicular(XYZ v)
        {
            if (v == null || v.IsZeroLength()) return null;

            XYZ candidate = Math.Abs(v.X) < 0.9 ? XYZ.BasisX : XYZ.BasisY;
            return v.CrossProduct(candidate).Normalize();
        }
    }
}