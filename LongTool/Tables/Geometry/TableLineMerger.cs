using System;
using System.Collections.Generic;
using System.Linq;

namespace LongTool.Tables.Geometry;


/// <summary>
/// Gộp các TableLine có cùng phương và liên tiếp.
/// Xử lý các đoạn line bị chia nhỏ do Merge Cell.
/// </summary>
public static class TableLineMerger
{
    private const double Tolerance = 0.000001;



    public static List<TableLine> Merge(
        IEnumerable<TableLine> lines)
    {
        if (lines == null)
            throw new ArgumentNullException(nameof(lines));


        var result =
            new List<TableLine>();


        var remaining =
            lines.ToList();



        while (remaining.Count > 0)
        {
            TableLine current =
                remaining[0];

            remaining.RemoveAt(0);


            bool merged;


            do
            {
                merged = false;


                for (int i = 0; i < remaining.Count; i++)
                {
                    TableLine other =
                        remaining[i];


                    if (CanMerge(current, other))
                    {
                        current =
                            MergeTwo(current, other);


                        remaining.RemoveAt(i);

                        merged = true;

                        break;
                    }

                }

            }
            while (merged);



            result.Add(current);
        }


        return result;
    }



    private static bool CanMerge(
        TableLine a,
        TableLine b)
    {
        if (Math.Abs(a.LineWidth - b.LineWidth) > Tolerance)
            return false;


        // Line đứng
        if (IsVertical(a) && IsVertical(b))
        {
            if (Math.Abs(a.Start.X - b.Start.X) > Tolerance)
                return false;


            double aMin = Math.Min(a.Start.Y, a.End.Y);
            double aMax = Math.Max(a.Start.Y, a.End.Y);

            double bMin = Math.Min(b.Start.Y, b.End.Y);
            double bMax = Math.Max(b.Start.Y, b.End.Y);


            // chỉ nối khi thật sự tiếp xúc hoặc chồng nhau
            return
                bMin <= aMax + Tolerance &&
                bMax >= aMin - Tolerance;
        }



        // Line ngang
        if (IsHorizontal(a) && IsHorizontal(b))
        {
            if (Math.Abs(a.Start.Y - b.Start.Y) > Tolerance)
                return false;


            double aMin = Math.Min(a.Start.X, a.End.X);
            double aMax = Math.Max(a.Start.X, a.End.X);

            double bMin = Math.Min(b.Start.X, b.End.X);
            double bMax = Math.Max(b.Start.X, b.End.X);


            return
                bMin <= aMax + Tolerance &&
                bMax >= aMin - Tolerance;
        }



        return false;
    }



    private static TableLine MergeTwo(
        TableLine a,
        TableLine b)
    {
        if (IsVertical(a))
        {
            double x = a.Start.X;


            double minY =
                Math.Min(
                    Math.Min(a.Start.Y, a.End.Y),
                    Math.Min(b.Start.Y, b.End.Y));


            double maxY =
                Math.Max(
                    Math.Max(a.Start.Y, a.End.Y),
                    Math.Max(b.Start.Y, b.End.Y));


            return new TableLine(
                new TablePoint(x, minY),
                new TablePoint(x, maxY),
                a.LineWidth);
        }



        else
        {
            double y = a.Start.Y;


            double minX =
                Math.Min(
                    Math.Min(a.Start.X, a.End.X),
                    Math.Min(b.Start.X, b.End.X));


            double maxX =
                Math.Max(
                    Math.Max(a.Start.X, a.End.X),
                    Math.Max(b.Start.X, b.End.X));


            return new TableLine(
                new TablePoint(minX, y),
                new TablePoint(maxX, y),
                a.LineWidth);
        }
    }



    private static bool IsVertical(
        TableLine line)
    {
        return
            Math.Abs(
                line.Start.X - line.End.X)
            < Tolerance;
    }



    private static bool IsHorizontal(
        TableLine line)
    {
        return
            Math.Abs(
                line.Start.Y - line.End.Y)
            < Tolerance;
    }
}