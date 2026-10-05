using Autodesk.Revit.DB;
using LongTool.Core.Geometry;
using LongTool.Services.Dimensions;
using LongTool.Services.Dimensions.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LongTool.Services.BeamDimension
{
    /// <summary>
    /// Service tạo dim mặt cắt cho dầm.
    /// Dim trực tiếp vào tâm dầm bằng reference plane có sẵn của FamilyInstance,
    /// KHÔNG cần tạo DetailCurve phụ.
    /// </summary>
    public class BeamSectionDimensionService
    {
        private readonly BeamDimensionOptions _opt;

        public BeamSectionDimensionService(BeamDimensionOptions opt = null)
        {
            _opt = opt ?? new BeamDimensionOptions();
        }

        // ============================================================
        // LỌC DẦM
        // ============================================================

        public List<FamilyInstance> FilterBeamsPerpendicular(
            IEnumerable<FamilyInstance> beams,
            View view)
        {
            List<FamilyInstance> result = new List<FamilyInstance>();

            if (beams == null || view == null)
                return result;

            foreach (FamilyInstance beam in beams)
            {
                if (IsPerpendicularToView(beam, view))
                    result.Add(beam);
            }

            return result;
        }

        public bool IsPerpendicularToView(
            FamilyInstance beam,
            View view)
        {
            if (beam == null || view == null)
                return false;

            LocationCurve lc = beam.Location as LocationCurve;

            if (lc == null)
                return false;

            XYZ dir =
                lc.Curve.GetEndPoint(1) -
                lc.Curve.GetEndPoint(0);

            if (dir.IsZeroLength())
                return false;

            dir = dir.Normalize();

            XYZ viewDir = view.ViewDirection.Normalize();

            double dot =
                Math.Abs(dir.DotProduct(viewDir));

            return dot > _opt.PerpendicularTolerance;
        }

        // ============================================================
        // TẠO DIM CHO 1 DẦM
        // ============================================================

        public bool CreateDimensions(
            Document doc,
            View view,
            FamilyInstance beam,
            DimensionType dimStyle)
        {
            if (doc == null ||
                view == null ||
                beam == null)
            {
                return false;
            }

            List<Solid> solids =
                SolidExtractor.GetAllSolidsSortedByVolume(beam);

            if (solids.Count == 0)
                return false;

            LinearElementFrame frame =
                LinearElementFrame.FromInstance(beam, view);

            if (frame == null)
                return false;

            foreach (Solid solid in solids)
            {
                if (TryCreateDimsForSolid(
                    doc,
                    view,
                    beam,
                    solid,
                    frame,
                    dimStyle))
                {
                    return true;
                }
            }

            return false;
        }

        // ============================================================
        // TẠO DIM CHO 1 SOLID
        // ============================================================

        private bool TryCreateDimsForSolid(
            Document doc,
            View view,
            FamilyInstance beam,
            Solid solid,
            LinearElementFrame frame,
            DimensionType dimStyle)
        {
            // ========================================================
            // 1. MẶT CẮT + TÂM
            // ========================================================

            PlanarFace sectionFace =
                FaceUtils.FindSectionFace(
                    solid,
                    frame.Axis);

            if (sectionFace == null)
                return false;

            XYZ center =
                FaceUtils.GetFaceCenter(sectionFace);

            if (center == null)
                return false;

            // ========================================================
            // 2. TÌM CẶP MẶT ĐỐI DIỆN
            // ========================================================

            PlanarFace widthFace1;
            PlanarFace widthFace2;
            PlanarFace heightFace1;
            PlanarFace heightFace2;

            if (!FaceUtils.FindOppositeFaces(
                solid,
                frame.Axis,
                frame.WidthDir,
                frame.HeightDir,
                out widthFace1,
                out widthFace2,
                out heightFace1,
                out heightFace2))
            {
                return false;
            }

            // ========================================================
            // 3. TÍNH KÍCH THƯỚC DẦM
            // ========================================================

            XYZ widthFace1Center =
                FaceUtils.GetFaceCenter(widthFace1);

            XYZ widthFace2Center =
                FaceUtils.GetFaceCenter(widthFace2);

            XYZ heightFace1Center =
                FaceUtils.GetFaceCenter(heightFace1);

            XYZ heightFace2Center =
                FaceUtils.GetFaceCenter(heightFace2);

            if (widthFace1Center == null ||
                widthFace2Center == null ||
                heightFace1Center == null ||
                heightFace2Center == null)
            {
                return false;
            }

            double halfWidth =
                Math.Abs(
                    (widthFace2Center - widthFace1Center)
                        .DotProduct(frame.WidthDir))
                / 2.0;

            double halfHeight =
                Math.Abs(
                    (heightFace2Center - heightFace1Center)
                        .DotProduct(frame.HeightDir))
                / 2.0;

            // ========================================================
            // 4. LAYOUT
            // ========================================================

            double legLength =
                DimensionStyleHelper.GetLegLength(dimStyle);

            DimensionLayoutOptions layoutOptions =
                new DimensionLayoutOptions
                {
                    GapFromObjectMm =
                        _opt.GapFromObjectMm,

                    MinHalfLineMm =
                        _opt.MinHalfLineMm,

                    HalfLineFactor =
                        _opt.HalfLineFactor,

                    Layer2OffsetFactor =
                        _opt.Layer2OffsetFactor
                };

            DimensionLayoutResult layout =
                DimensionLayoutCalculator.Compute(
                    center,
                    frame,
                    halfWidth,
                    halfHeight,
                    legLength,
                    layoutOptions);

            if (layout == null)
                return false;

            // ========================================================
            // 5. HEIGHT DIM
            // ========================================================

            Dimension dimH =
                DimensionFactory.TryCreate(
                    doc,
                    view,
                    heightFace1,
                    heightFace2,
                    layout.HeightDimPosition,
                    layout.HeightDimDirection,
                    layout.HalfLine,
                    dimStyle);

            System.Diagnostics.Debug.WriteLine(
                $"Height dim: {(dimH != null ? "OK" : "FAIL")}");

            // ========================================================
            // 6. LẤY REFERENCE TÂM DẦM TRỰC TIẾP
            //
            // Dùng reference plane có sẵn của FamilyInstance,
            // KHÔNG tạo DetailCurve phụ.
            // ========================================================

            Reference centerRef =
                GetBeamCenterReference(beam, frame.WidthDir);

            System.Diagnostics.Debug.WriteLine(
                $"Center reference: " +
                $"{(centerRef != null ? "OK" : "NULL")}");

            if (centerRef == null)
                return false;

            // ========================================================
            // 7. DIM NỬA TRÁI
            // ========================================================

            Dimension dimL =
                DimensionFactory.TryCreate(
                    doc,
                    view,
                    widthFace1.Reference,
                    centerRef,
                    layout.WidthLayer1Position,
                    layout.WidthLayer1Direction,
                    layout.HalfLine,
                    dimStyle);

            System.Diagnostics.Debug.WriteLine(
                $"Dim left: {(dimL != null ? "OK" : "FAIL")}");

            // ========================================================
            // 8. DIM NỬA PHẢI
            // ========================================================

            Dimension dimR =
                DimensionFactory.TryCreate(
                    doc,
                    view,
                    centerRef,
                    widthFace2.Reference,
                    layout.WidthLayer1Position,
                    layout.WidthLayer1Direction,
                    layout.HalfLine,
                    dimStyle);

            System.Diagnostics.Debug.WriteLine(
                $"Dim right: {(dimR != null ? "OK" : "FAIL")}");

            // ========================================================
            // 9. DIM TỔNG - LAYER 2
            // ========================================================

            Dimension dim2 =
                DimensionFactory.TryCreate(
                    doc,
                    view,
                    widthFace1,
                    widthFace2,
                    layout.WidthLayer2Position,
                    layout.WidthLayer2Direction,
                    layout.HalfLine,
                    dimStyle);

            System.Diagnostics.Debug.WriteLine(
                $"Dim layer2: {(dim2 != null ? "OK" : "FAIL")}");

            // ========================================================
            // KẾT QUẢ
            // ========================================================

            return
                dimH != null ||
                dimL != null ||
                dimR != null ||
                dim2 != null;
        }

        // ============================================================
        // LẤY REFERENCE TÂM DẦM (KHÔNG TẠO DETAIL CURVE)
        // ============================================================

        /// <summary>
        /// Lấy reference plane tâm của dầm, ưu tiên reference có normal
        /// cùng/ngược hướng với widthDir.
        /// </summary>
        private Reference GetBeamCenterReference(
            FamilyInstance beam,
            XYZ widthDir)
        {
            if (beam == null || widthDir == null)
                return null;

            widthDir = widthDir.Normalize();

            List<Reference> candidates = new List<Reference>();

            foreach (FamilyInstanceReferenceType t in new[]
            {
                FamilyInstanceReferenceType.CenterFrontBack,
                FamilyInstanceReferenceType.CenterLeftRight,
                FamilyInstanceReferenceType.CenterElevation
            })
            {
                IList<Reference> refs = beam.GetReferences(t);

                if (refs == null)
                    continue;

                foreach (Reference r in refs)
                {
                    if (r != null)
                        candidates.Add(r);
                }
            }

            if (candidates.Count == 0)
                return null;

            // Ưu tiên reference có normal trùng widthDir
            foreach (Reference r in candidates)
            {
                XYZ normal = GetReferenceNormal(beam, r);

                if (normal == null)
                    continue;

                double dot =
                    Math.Abs(normal.DotProduct(widthDir));

                if (dot > 0.99)
                    return r;
            }

            // Fallback: reference đầu tiên (thường là CenterFrontBack)
            return candidates[0];
        }

        // ============================================================
        // LẤY NORMAL CỦA REFERENCE
        // ============================================================

        private XYZ GetReferenceNormal(
            FamilyInstance beam,
            Reference reference)
        {
            if (beam == null || reference == null)
                return null;

            Options opt = new Options
            {
                ComputeReferences = true,
                IncludeNonVisibleObjects = true,
                DetailLevel = ViewDetailLevel.Fine
            };

            GeometryElement geo = beam.get_Geometry(opt);

            if (geo == null)
                return null;

            string refStable =
                reference.ConvertToStableRepresentation(
                    beam.Document);

            foreach (GeometryObject obj in geo)
            {
                XYZ normal = TryGetNormalFromObject(
                    obj, reference, refStable, beam.Document);

                if (normal != null)
                    return normal;
            }

            return null;
        }

        private XYZ TryGetNormalFromObject(
            GeometryObject obj,
            Reference reference,
            string refStable,
            Document doc)
        {
            if (obj is Solid solid)
            {
                foreach (Face f in solid.Faces)
                {
                    if (f.Reference == null)
                        continue;

                    string s =
                        f.Reference.ConvertToStableRepresentation(doc);

                    if (s == refStable)
                    {
                        return f.ComputeNormal(new UV(0.5, 0.5))
                                .Normalize();
                    }
                }
            }
            else if (obj is GeometryInstance gi)
            {
                GeometryElement instGeo = gi.GetInstanceGeometry();

                if (instGeo == null)
                    return null;

                foreach (GeometryObject io in instGeo)
                {
                    XYZ n = TryGetNormalFromObject(
                        io, reference, refStable, doc);

                    if (n != null)
                        return n;
                }
            }

            return null;
        }
    }
}