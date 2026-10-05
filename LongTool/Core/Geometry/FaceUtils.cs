using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace LongTool.Core.Geometry
{
    /// <summary>
    /// Các hàm xử lý PlanarFace: tìm mặt, tâm mặt, chọn mặt cực trị,
    /// tìm reference tâm...
    /// </summary>
    public static class FaceUtils
    {
        // Ngưỡng mặc định
        public const double AXIS_TOLERANCE = 0.90;
        public const double FACE_DIRECTION_TOLERANCE = 0.85;

        // ============================================================
        // LẤY TÂM MẶT
        // ============================================================

        public static XYZ GetFaceCenter(PlanarFace face)
        {
            if (face == null) return null;

            try
            {
                BoundingBoxUV bb = face.GetBoundingBox();
                if (bb == null) return null;

                UV mid = (bb.Min + bb.Max) * 0.5;
                return face.Evaluate(mid);
            }
            catch
            {
                return null;
            }
        }

        // ============================================================
        // TÌM MẶT CẮT NGANG (VUÔNG GÓC AXIS)
        // ============================================================

        public static PlanarFace FindSectionFace(
            Solid solid,
            XYZ axis,
            double tolerance = AXIS_TOLERANCE)
        {
            if (solid == null || axis == null) return null;

            PlanarFace result = null;
            double bestDot = 0;

            foreach (Face f in solid.Faces)
            {
                PlanarFace pf = f as PlanarFace;
                if (pf == null) continue;

                double dot = Math.Abs(
                    pf.FaceNormal.Normalize().DotProduct(axis));

                if (dot > bestDot)
                {
                    bestDot = dot;
                    result = pf;
                }
            }

            return bestDot >= tolerance ? result : null;
        }

        // ============================================================
        // TÌM 2 CẶP MẶT ĐỐI DIỆN (WIDTH + HEIGHT)
        // ============================================================

        public static bool FindOppositeFaces(
            Solid solid,
            XYZ beamAxis,
            XYZ widthDir,
            XYZ heightDir,
            out PlanarFace widthFace1,
            out PlanarFace widthFace2,
            out PlanarFace heightFace1,
            out PlanarFace heightFace2,
            double axisTolerance = AXIS_TOLERANCE,
            double faceTolerance = FACE_DIRECTION_TOLERANCE)
        {
            widthFace1 = widthFace2 = null;
            heightFace1 = heightFace2 = null;

            if (solid == null) return false;

            List<PlanarFace> widthFaces = new List<PlanarFace>();
            List<PlanarFace> heightFaces = new List<PlanarFace>();

            foreach (Face f in solid.Faces)
            {
                PlanarFace pf = f as PlanarFace;
                if (pf == null) continue;
                if (pf.Reference == null) continue;

                XYZ n = pf.FaceNormal.Normalize();

                double dotAxis = Math.Abs(n.DotProduct(beamAxis));
                if (dotAxis > axisTolerance) continue;

                double dotWidth = Math.Abs(n.DotProduct(widthDir));
                double dotHeight = Math.Abs(n.DotProduct(heightDir));

                if (dotWidth > faceTolerance && dotWidth > dotHeight)
                    widthFaces.Add(pf);
                else if (dotHeight > faceTolerance)
                    heightFaces.Add(pf);
            }

            if (widthFaces.Count < 2 || heightFaces.Count < 2)
                return false;

            PickExtremeFaces(widthFaces, widthDir,
                out widthFace1, out widthFace2);

            PickExtremeFaces(heightFaces, heightDir,
                out heightFace1, out heightFace2);

            return widthFace1 != null && widthFace2 != null &&
                   heightFace1 != null && heightFace2 != null;
        }

        // ============================================================
        // CHỌN 2 MẶT XA NHAU NHẤT THEO AXIS
        // ============================================================

        public static void PickExtremeFaces(
            List<PlanarFace> faces,
            XYZ axis,
            out PlanarFace f1,
            out PlanarFace f2)
        {
            f1 = null;
            f2 = null;

            if (faces == null || faces.Count < 2) return;

            var ordered = faces
                .Select(f =>
                {
                    XYZ c = GetFaceCenter(f);
                    double t = c == null ? 0 : c.DotProduct(axis);
                    return new { Face = f, T = t };
                })
                .OrderBy(x => x.T)
                .ToList();

            f1 = ordered.First().Face;
            f2 = ordered.Last().Face;
        }

        // ============================================================
        // TÌM REFERENCE TÂM (MẶT SONG SONG DIR QUA CENTER)
        // ============================================================

        public static Reference FindCenterReference(
            Solid solid,
            XYZ center,
            XYZ dir,
            double parallelTolerance = 0.95,
            double distanceTolerance = 1e-6)
        {
            if (solid == null || center == null || dir == null)
                return null;

            PlanarFace best = null;
            double bestDist = double.MaxValue;

            foreach (Face f in solid.Faces)
            {
                PlanarFace pf = f as PlanarFace;
                if (pf == null) continue;
                if (pf.Reference == null) continue;

                XYZ n = pf.FaceNormal.Normalize();
                double dot = Math.Abs(n.DotProduct(dir));
                if (dot < parallelTolerance) continue;

                XYZ ptOnFace = GetFaceCenter(pf);
                if (ptOnFace == null) continue;

                double dist = Math.Abs(
                    (center - ptOnFace).DotProduct(n));

                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = pf;
                }
            }

            if (best == null) return null;
            if (bestDist > distanceTolerance) return null;

            return best.Reference;
        }
    }
}