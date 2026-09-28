using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.Services.DimStyle;

namespace LongTool.Commands.JohAbroad
{
    [Transaction(TransactionMode.Manual)]
    public class BeamSectionDimensionCommand : IExternalCommand
    {
        // Ngưỡng nới lỏng để chấp nhận dầm slope/xoay nhẹ
        private const double PERPENDICULAR_TOLERANCE = 0.95;   // ~18°
        private const double FACE_DIRECTION_TOLERANCE = 0.85;  // ~32°

        // Khoảng cách chân dim đến mặt đối tượng (mm)
        private const double GAP_FROM_OBJECT_MM = 2.0;

        // Fallback nếu không đọc được chân dim từ style
        private const double FALLBACK_LEG_LENGTH_MM = 2.5;

        // Độ dài nửa đường dim (mm) - phải đủ dài để phủ hết đối tượng
        private const double HALF_LINE_MM = 150.0;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            View view = doc.ActiveView;

            if (view == null)
            {
                message = "Không tìm thấy View hiện tại.";
                return Result.Failed;
            }

            // ========================================================
            // 1. COLLECT BEAMS
            // ========================================================

            List<FamilyInstance> beams =
                new FilteredElementCollector(doc, view.Id)
                    .OfCategory(BuiltInCategory.OST_StructuralFraming)
                    .WhereElementIsNotElementType()
                    .OfType<FamilyInstance>()
                    .ToList();

            // ========================================================
            // 2. FILTER TARGET BEAMS
            // ========================================================

            List<FamilyInstance> targetBeams = new List<FamilyInstance>();

            foreach (FamilyInstance beam in beams)
            {
                if (!IsBeamPerpendicularToView(beam, view))
                    continue;

                targetBeams.Add(beam);
            }

            if (targetBeams.Count == 0)
            {
                TaskDialog.Show(
                    "LongTool - Beam Dimension",
                    "Không tìm thấy dầm vuông góc với mặt phẳng bản vẽ.");

                return Result.Succeeded;
            }

            // ========================================================
            // 3. SELECT DIM STYLE
            // ========================================================

            DimStyleSelectorService dimStyleService =
                new DimStyleSelectorService();

            DimensionType dimStyle =
                dimStyleService.Select(doc);

            if (dimStyle == null)
                return Result.Cancelled;

            // ========================================================
            // 4. CREATE DIMENSIONS
            // ========================================================

            int successCount = 0;
            List<string> failedInfo = new List<string>();

            using (Transaction transaction =
                new Transaction(doc, "LongTool - Dimension Beam Sections"))
            {
                transaction.Start();

                foreach (FamilyInstance beam in targetBeams)
                {
                    try
                    {
                        if (CreateBeamDimensions(doc, view, beam, dimStyle))
                        {
                            successCount++;
                        }
                        else
                        {
                            failedInfo.Add(
                                $"Beam ID {beam.Id.Value}: không tạo được dim.");
                        }
                    }
                    catch (Exception ex)
                    {
                        failedInfo.Add(
                            $"Beam ID {beam.Id.Value}: " +
                            $"{ex.GetType().Name} - {ex.Message}");
                    }
                }

                transaction.Commit();
            }

            // ========================================================
            // 5. RESULT
            // ========================================================

            string msg =
                $"Dim Style: {dimStyle.Name}\n" +
                $"Dầm tìm được: {targetBeams.Count}\n" +
                $"Dầm đã Dim: {successCount}";

            if (failedInfo.Count > 0)
            {
                msg += "\n\nLỗi:\n" +
                       string.Join("\n", failedInfo.Take(10));

                if (failedInfo.Count > 10)
                {
                    msg += $"\n... và {failedInfo.Count - 10} dầm khác.";
                }
            }

            TaskDialog.Show("LongTool - Beam Dimension", msg);

            return Result.Succeeded;
        }

        // ============================================================
        // BEAM AXIS PERPENDICULAR TO VIEW
        // ============================================================

        private static bool IsBeamPerpendicularToView(
            FamilyInstance beam,
            View view)
        {
            LocationCurve lc = beam.Location as LocationCurve;
            if (lc == null)
                return false;

            XYZ dir = lc.Curve.GetEndPoint(1) - lc.Curve.GetEndPoint(0);
            if (dir.IsZeroLength())
                return false;

            dir = dir.Normalize();

            XYZ viewDir = view.ViewDirection.Normalize();
            double dot = Math.Abs(dir.DotProduct(viewDir));

            return dot > PERPENDICULAR_TOLERANCE;
        }

        // ============================================================
        // CREATE DIMENSIONS - ROBUST VERSION
        // ============================================================

        private static bool CreateBeamDimensions(
            Document doc,
            View view,
            FamilyInstance beam,
            DimensionType dimStyle)
        {
            Options opt = new Options
            {
                ComputeReferences = true,
                IncludeNonVisibleObjects = false,
                DetailLevel = ViewDetailLevel.Fine
            };

            GeometryElement geometry = beam.get_Geometry(opt);
            if (geometry == null)
                return false;

            List<Solid> solids = FindAllSolids(geometry, minVolume: 1e-6);
            if (solids.Count == 0)
                return false;

            solids = solids.OrderByDescending(s => s.Volume).ToList();

            XYZ beamAxis = GetBeamAxis(beam, view);
            if (beamAxis == null)
                return false;

            // Hệ trục local của dầm
            XYZ widthDir = ComputeWidthDirection(beamAxis, view);
            XYZ heightDir = beamAxis.CrossProduct(widthDir).Normalize();

            // Đảm bảo heightDir hướng lên trên (cùng chiều view.Up)
            if (heightDir.DotProduct(view.UpDirection) < 0)
                heightDir = heightDir.Negate();

            foreach (Solid solid in solids)
            {
                if (TryCreateDimsForSolid(
                        doc, view,
                        solid,
                        beamAxis, widthDir, heightDir,
                        dimStyle))
                {
                    return true;
                }
            }

            return false;
        }

        // ============================================================
        // THỬ TẠO DIM CHO 1 SOLID
        // ============================================================

        private static bool TryCreateDimsForSolid(
            Document doc,
            View view,
            Solid solid,
            XYZ beamAxis,
            XYZ widthDir,
            XYZ heightDir,
            DimensionType dimStyle)
        {
            // 1. Tìm mặt cắt
            PlanarFace sectionFace = FindSectionFace(solid, beamAxis);
            if (sectionFace == null)
                return false;

            // 2. Tâm mặt cắt
            XYZ center = GetFaceCenter(sectionFace);
            if (center == null)
                return false;

            // 3. Tìm cặp mặt bên
            PlanarFace widthFace1, widthFace2;
            PlanarFace heightFace1, heightFace2;

            if (!FindOppositeFaces(
                    solid, beamAxis,
                    widthDir, heightDir,
                    out widthFace1, out widthFace2,
                    out heightFace1, out heightFace2))
            {
                return false;
            }

            // 4. Tính toán khoảng cách
            double gapFromObject = UnitUtils.ConvertToInternalUnits(
                GAP_FROM_OBJECT_MM, UnitTypeId.Millimeters);

            double legLength = GetDimLegLength(dimStyle);

            double dimLineDistance = gapFromObject + legLength;
            double halfLine = UnitUtils.ConvertToInternalUnits(
                HALF_LINE_MM, UnitTypeId.Millimeters);

            // 5. Vị trí mặt ngoài đối tượng
            XYZ widthFace1Center = GetFaceCenter(widthFace1);
            XYZ widthFace2Center = GetFaceCenter(widthFace2);
            XYZ heightFace1Center = GetFaceCenter(heightFace1);
            XYZ heightFace2Center = GetFaceCenter(heightFace2);

            if (widthFace1Center == null || widthFace2Center == null ||
                heightFace1Center == null || heightFace2Center == null)
                return false;

            // Chiều rộng và chiều cao dầm (khoảng cách giữa 2 mặt)
            double halfWidth = Math.Abs(
                (widthFace2Center - widthFace1Center)
                    .DotProduct(widthDir)) / 2.0;

            double halfHeight = Math.Abs(
                (heightFace2Center - heightFace1Center)
                    .DotProduct(heightDir)) / 2.0;

            // Mặt ngoài cùng bên phải và mặt dưới cùng của dầm
            XYZ rightFacePos = center + widthDir.Multiply(halfWidth);
            XYZ bottomFacePos = center - heightDir.Multiply(halfHeight);

            // ============================================================
            // 6. HEIGHT DIMENSION - BÊN PHẢI, CÁCH 2MM
            // ============================================================

            XYZ heightDimPos = rightFacePos +
                               widthDir.Multiply(dimLineDistance);

            TryCreateOneDim(
                doc, view,
                heightFace1, heightFace2,
                heightDimPos,
                heightDir,
                halfLine,
                dimStyle);

            // ============================================================
            // 7. WIDTH DIMENSION - 2 LỚP Ở DƯỚI
            // ============================================================

            // --- Lớp 1: 2 dim (trái + phải) qua tâm ---
            XYZ widthLayer1Pos = bottomFacePos -
                                 heightDir.Multiply(dimLineDistance);

            // Tìm reference tâm (mặt phẳng song song widthFace đi qua center)
            Reference centerRef = FindCenterReference(
                solid, center, widthDir);

            if (centerRef != null)
            {
                // Dim trái: widthFace1 -> center
                TryCreateOneDim(
                    doc, view,
                    widthFace1.Reference, centerRef,
                    widthLayer1Pos,
                    widthDir,
                    halfLine,
                    dimStyle);

                // Dim phải: center -> widthFace2
                TryCreateOneDim(
                    doc, view,
                    centerRef, widthFace2.Reference,
                    widthLayer1Pos,
                    widthDir,
                    halfLine,
                    dimStyle);
            }
            else
            {
                // Fallback: 1 dim full width ở lớp 1
                TryCreateOneDim(
                    doc, view,
                    widthFace1, widthFace2,
                    widthLayer1Pos,
                    widthDir,
                    halfLine,
                    dimStyle);
            }

            // --- Lớp 2: dim full width, cách lớp 1 = legLength ---
            XYZ widthLayer2Pos = widthLayer1Pos -
                                 heightDir.Multiply(legLength);

            TryCreateOneDim(
                doc, view,
                widthFace1, widthFace2,
                widthLayer2Pos,
                widthDir,
                halfLine,
                dimStyle);

            return true;
        }

        // ============================================================
        // TÍNH CHIỀU DÀI CHÂN DIM
        // ============================================================

        private static double GetDimLegLength(DimensionType dimStyle)
        {
            double fallback = UnitUtils.ConvertToInternalUnits(
                FALLBACK_LEG_LENGTH_MM, UnitTypeId.Millimeters);

            if (dimStyle == null)
                return fallback;

            double textOffset = 0;
            double textSize = 0;

            // Text Offset - truy xuất qua tên chuỗi, không phải BuiltInParameter
            try
            {
                Parameter pOffset = dimStyle.LookupParameter("Text Offset");
                if (pOffset != null && pOffset.HasValue)
                    textOffset = pOffset.AsDouble();
            }
            catch { }

            // Text Size - có thể dùng BuiltInParameter.TEXT_SIZE (vẫn tồn tại)
            try
            {
                Parameter pSize = dimStyle.get_Parameter(
                    BuiltInParameter.TEXT_SIZE);
                if (pSize != null && pSize.HasValue)
                    textSize = pSize.AsDouble();
            }
            catch { }

            double total = textOffset + textSize;
            return total > 1e-9 ? total : fallback;
        }

        // ============================================================
        // TÌM REFERENCE TÂM
        // (MẶT PHẲNG SONG SONG WIDTHDIR QUA CENTER)
        // ============================================================

        private static Reference FindCenterReference(
            Solid solid,
            XYZ center,
            XYZ widthDir)
        {
            PlanarFace best = null;
            double bestDist = double.MaxValue;

            foreach (Face f in solid.Faces)
            {
                PlanarFace pf = f as PlanarFace;
                if (pf == null) continue;
                if (pf.Reference == null) continue;

                XYZ n = pf.FaceNormal.Normalize();
                double dot = Math.Abs(n.DotProduct(widthDir));
                if (dot < 0.95) continue;

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
            if (bestDist > 1e-6) return null;

            return best.Reference;
        }

        // ============================================================
        // TẠO 1 DIMENSION TỪ 2 REFERENCE
        // ============================================================

        private static bool TryCreateOneDim(
            Document doc,
            View view,
            Reference ref1,
            Reference ref2,
            XYZ dimPosition,
            XYZ dimDirection,
            double halfLine,
            DimensionType dimStyle)
        {
            if (ref1 == null || ref2 == null)
                return false;

            try
            {
                if (ref1.ElementId == ref2.ElementId &&
                    ref1.ConvertToStableRepresentation(doc) ==
                    ref2.ConvertToStableRepresentation(doc))
                {
                    return false;
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
                    return false;

                if (dimStyle != null &&
                    dim.DimensionType != null &&
                    dim.DimensionType.Id != dimStyle.Id &&
                    IsLinearDimensionType(dimStyle))
                {
                    try { dim.DimensionType = dimStyle; }
                    catch { }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // ============================================================
        // TẠO 1 DIMENSION TỪ 2 PLANARFACE
        // ============================================================

        private static bool TryCreateOneDim(
            Document doc,
            View view,
            PlanarFace face1,
            PlanarFace face2,
            XYZ dimPosition,
            XYZ dimDirection,
            double halfLine,
            DimensionType dimStyle)
        {
            if (face1 == null || face2 == null)
                return false;

            if (face1.Reference == null || face2.Reference == null)
                return false;

            XYZ n1 = face1.FaceNormal.Normalize();
            XYZ n2 = face2.FaceNormal.Normalize();
            double dotNormals = Math.Abs(n1.DotProduct(n2));
            if (dotNormals < 0.90)
                return false;

            return TryCreateOneDim(
                doc, view,
                face1.Reference, face2.Reference,
                dimPosition, dimDirection, halfLine,
                dimStyle);
        }

        // ============================================================
        // KIỂM TRA DIMENSION TYPE CÓ PHẢI LINEAR
        // ============================================================

        private static bool IsLinearDimensionType(DimensionType dt)
        {
            if (dt == null)
                return false;

            return dt.StyleType == DimensionStyleType.Linear
                || dt.StyleType == DimensionStyleType.LinearFixed;
        }

        // ============================================================
        // TÍNH TRỤC DẦM (WORLD)
        // ============================================================

        private static XYZ GetBeamAxis(FamilyInstance beam, View view)
        {
            LocationCurve lc = beam.Location as LocationCurve;
            if (lc == null)
                return null;

            XYZ dir = lc.Curve.GetEndPoint(1) - lc.Curve.GetEndPoint(0);
            if (dir.IsZeroLength())
                return null;

            dir = dir.Normalize();

            XYZ viewDir = view.ViewDirection.Normalize();
            if (dir.DotProduct(viewDir) < 0)
                dir = dir.Negate();

            return dir;
        }

        // ============================================================
        // TÍNH WIDTH DIRECTION (PHƯƠNG NGANG MẶT CẮT)
        // ============================================================

        private static XYZ ComputeWidthDirection(XYZ beamAxis, View view)
        {
            XYZ viewRight = view.RightDirection.Normalize();

            XYZ projected = viewRight -
                            beamAxis.Multiply(viewRight.DotProduct(beamAxis));

            if (!projected.IsZeroLength())
                return projected.Normalize();

            XYZ viewUp = view.UpDirection.Normalize();
            projected = viewUp -
                        beamAxis.Multiply(viewUp.DotProduct(beamAxis));

            if (!projected.IsZeroLength())
                return projected.Normalize();

            XYZ xAxis = XYZ.BasisX;
            projected = xAxis -
                        beamAxis.Multiply(xAxis.DotProduct(beamAxis));

            if (!projected.IsZeroLength())
                return projected.Normalize();

            return GetPerpendicular(beamAxis);
        }

        // ============================================================
        // LẤY VECTOR VUÔNG GÓC BẤT KỲ
        // ============================================================

        private static XYZ GetPerpendicular(XYZ v)
        {
            XYZ candidate = Math.Abs(v.X) < 0.9 ? XYZ.BasisX : XYZ.BasisY;
            XYZ perp = v.CrossProduct(candidate);
            return perp.Normalize();
        }

        // ============================================================
        // TÌM MẶT CẮT NGANG
        // ============================================================

        private static PlanarFace FindSectionFace(
            Solid solid,
            XYZ beamAxis)
        {
            PlanarFace result = null;
            double bestDot = 0;

            foreach (Face face in solid.Faces)
            {
                PlanarFace pf = face as PlanarFace;
                if (pf == null)
                    continue;

                double dot =
                    Math.Abs(pf.FaceNormal.Normalize().DotProduct(beamAxis));

                if (dot > bestDot)
                {
                    bestDot = dot;
                    result = pf;
                }
            }

            if (bestDot < 0.90)
                return null;

            return result;
        }

        // ============================================================
        // TÌM 2 CẶP MẶT ĐỐI DIỆN
        // ============================================================

        private static bool FindOppositeFaces(
            Solid solid,
            XYZ beamAxis,
            XYZ widthDir,
            XYZ heightDir,
            out PlanarFace widthFace1,
            out PlanarFace widthFace2,
            out PlanarFace heightFace1,
            out PlanarFace heightFace2)
        {
            widthFace1 = widthFace2 = null;
            heightFace1 = heightFace2 = null;

            List<PlanarFace> widthFaces = new List<PlanarFace>();
            List<PlanarFace> heightFaces = new List<PlanarFace>();

            foreach (Face f in solid.Faces)
            {
                PlanarFace pf = f as PlanarFace;
                if (pf == null) continue;
                if (pf.Reference == null) continue;

                XYZ n = pf.FaceNormal.Normalize();

                double dotAxis = Math.Abs(n.DotProduct(beamAxis));
                if (dotAxis > 0.90) continue;

                double dotWidth = Math.Abs(n.DotProduct(widthDir));
                double dotHeight = Math.Abs(n.DotProduct(heightDir));

                if (dotWidth > FACE_DIRECTION_TOLERANCE &&
                    dotWidth > dotHeight)
                {
                    widthFaces.Add(pf);
                }
                else if (dotHeight > FACE_DIRECTION_TOLERANCE)
                {
                    heightFaces.Add(pf);
                }
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

        private static void PickExtremeFaces(
            List<PlanarFace> faces,
            XYZ axis,
            out PlanarFace f1,
            out PlanarFace f2)
        {
            f1 = null;
            f2 = null;

            var ordered = faces
                .Select(f =>
                {
                    XYZ c = GetFaceCenter(f);
                    double t = c == null ? 0 : c.DotProduct(axis);
                    return new { Face = f, T = t };
                })
                .OrderBy(x => x.T)
                .ToList();

            if (ordered.Count >= 2)
            {
                f1 = ordered.First().Face;
                f2 = ordered.Last().Face;
            }
        }

        // ============================================================
        // LẤY TÂM MẶT
        // ============================================================

        private static XYZ GetFaceCenter(PlanarFace face)
        {
            try
            {
                BoundingBoxUV bb = face.GetBoundingBox();
                if (bb == null)
                    return null;

                UV mid = (bb.Min + bb.Max) * 0.5;
                return face.Evaluate(mid);
            }
            catch
            {
                return null;
            }
        }

        // ============================================================
        // LẤY TẤT CẢ SOLID HỢP LỆ
        // ============================================================

        private static List<Solid> FindAllSolids(
            GeometryElement geometry,
            double minVolume)
        {
            List<Solid> result = new List<Solid>();

            foreach (GeometryObject obj in geometry)
            {
                Solid solid = obj as Solid;
                if (solid == null) continue;
                if (solid.Volume <= minVolume) continue;

                result.Add(solid);
            }

            return result;
        }
    }
}