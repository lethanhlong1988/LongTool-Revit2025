namespace LongTool.Core.Geometry;

/// <summary>
/// Đại diện cho một điểm trong không gian 3D.
/// </summary>
public sealed class Point3D
{
    public double X { get; }

    public double Y { get; }

    public double Z { get; }

    public Point3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}