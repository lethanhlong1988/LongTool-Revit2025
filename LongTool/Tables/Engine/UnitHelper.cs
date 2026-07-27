using Autodesk.Revit.DB;

namespace LongTool.Tables.Engine;

public static class UnitHelper
{
    /// <summary>
    /// Convert millimeter to Revit internal unit (Feet)
    /// </summary>
    public static double Mm(double value)
    {
        return UnitUtils.ConvertToInternalUnits(
            value,
            UnitTypeId.Millimeters);
    }

    /// <summary>
    /// Convert centimeter to Feet
    /// </summary>
    public static double Cm(double value)
    {
        return Mm(value * 10);
    }

    /// <summary>
    /// Convert meter to Feet
    /// </summary>
    public static double M(double value)
    {
        return Mm(value * 1000);
    }
}