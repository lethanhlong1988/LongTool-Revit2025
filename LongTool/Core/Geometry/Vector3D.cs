using System;

namespace LongTool.Core.Geometry;

/// <summary>
/// Đại diện cho một vector trong không gian 3D.
/// </summary>
public sealed class Vector3D
{
    public double X { get; }

    public double Y { get; }

    public double Z { get; }

    /// <summary>
    /// Độ dài của vector.
    /// </summary>
    public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

    public Vector3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Trả về vector đơn vị.
    /// </summary>
    public Vector3D Normalize()
    {
        if (Length == 0)
            throw new InvalidOperationException("Cannot normalize a zero vector.");

        return new Vector3D(
            X / Length,
            Y / Length,
            Z / Length);
    }

    /// <summary>
    /// Tích vô hướng (Dot Product).
    /// </summary>
    public double Dot(Vector3D other)
    {
        return
            X * other.X +
            Y * other.Y +
            Z * other.Z;
    }

    /// <summary>
    /// Tích có hướng (Cross Product).
    /// </summary>
    public Vector3D Cross(Vector3D other)
    {
        return new Vector3D(
            Y * other.Z - Z * other.Y,
            Z * other.X - X * other.Z,
            X * other.Y - Y * other.X);
    }

    /// <summary>
    /// Cộng hai vector.
    /// </summary>
    public Vector3D Add(Vector3D other)
    {
        return new Vector3D(
            X + other.X,
            Y + other.Y,
            Z + other.Z);
    }

    /// <summary>
    /// Trừ hai vector.
    /// </summary>
    public Vector3D Subtract(Vector3D other)
    {
        return new Vector3D(
            X - other.X,
            Y - other.Y,
            Z - other.Z);
    }

    /// <summary>
    /// Nhân vector với một hệ số.
    /// </summary>
    public Vector3D Multiply(double scalar)
    {
        return new Vector3D(
            X * scalar,
            Y * scalar,
            Z * scalar);
    }
}