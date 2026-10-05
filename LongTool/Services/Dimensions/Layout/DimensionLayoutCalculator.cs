using System;
using Autodesk.Revit.DB;
using LongTool.Core.Geometry;

namespace LongTool.Services.Dimensions.Layout
{
    /// <summary>
    /// Tính toán vị trí đặt dim từ kích thước hình học + options.
    /// Thuần toán, không đụng tới Element / Solid.
    /// </summary>
    public static class DimensionLayoutCalculator
    {
        /// <summary>
        /// Tính layout cho 1 mặt cắt dầm.
        /// </summary>
        /// <param name="center">Tâm mặt cắt</param>
        /// <param name="frame">Hệ trục local của dầm</param>
        /// <param name="halfWidth">Nửa chiều rộng dầm</param>
        /// <param name="halfHeight">Nửa chiều cao dầm</param>
        /// <param name="legLength">Leg length (internal units), đọc từ style</param>
        /// <param name="options">Quy cách dim</param>
        public static DimensionLayoutResult Compute(
            XYZ center,
            LinearElementFrame frame,
            double halfWidth,
            double halfHeight,
            double legLength,
            DimensionLayoutOptions options)
        {
            if (center == null || frame == null || options == null)
                return null;

            // -------------------- Offset --------------------
            double gap = options.GapFromObjectInternal;
            double dimLineOffset = gap + legLength;
            double layer2Offset = legLength * options.Layer2OffsetFactor;

            // -------------------- Half line --------------------
            double maxHalfSize = Math.Max(halfWidth, halfHeight);
            double halfLine = Math.Max(
                maxHalfSize * options.HalfLineFactor,
                options.MinHalfLineInternal);

            // -------------------- Vị trí mặt ngoài dầm --------------------
            XYZ rightFacePos = center + frame.WidthDir.Multiply(halfWidth);
            XYZ bottomFacePos = center - frame.HeightDir.Multiply(halfHeight);

            // -------------------- Height dim (bên phải) --------------------
            XYZ heightPos = rightFacePos +
                            frame.WidthDir.Multiply(dimLineOffset);

            // -------------------- Width dim lớp 1 (dưới dầm) --------------------
            XYZ widthLayer1Pos = bottomFacePos -
                                 frame.HeightDir.Multiply(dimLineOffset);

            // -------------------- Width dim lớp 2 (dưới lớp 1) --------------------
            XYZ widthLayer2Pos = widthLayer1Pos -
                                 frame.HeightDir.Multiply(layer2Offset);

            // -------------------- Kết quả --------------------
            return new DimensionLayoutResult
            {
                HeightDimPosition = heightPos,
                HeightDimDirection = frame.HeightDir,

                WidthLayer1Position = widthLayer1Pos,
                WidthLayer1Direction = frame.WidthDir,

                WidthLayer2Position = widthLayer2Pos,
                WidthLayer2Direction = frame.WidthDir,

                HalfLine = halfLine,
                Layer2Offset = layer2Offset
            };
        }
    }
}