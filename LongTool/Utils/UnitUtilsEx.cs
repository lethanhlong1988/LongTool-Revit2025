using Autodesk.Revit.DB;

namespace LongTool.Utils;

public static class UnitUtilsEx
{
    /// <summary>
    /// Millimeters → Revit Internal Units (Feet)
    /// </summary>
    public static double MmToInternal(double mm)
    {
        return UnitUtils.ConvertToInternalUnits(
            mm,
            UnitTypeId.Millimeters);
    }

    /// <summary>
    /// Revit Internal Units (Feet) → Millimeters
    /// </summary>
    public static double InternalToMm(double value)
    {
        return UnitUtils.ConvertFromInternalUnits(
            value,
            UnitTypeId.Millimeters);
    }
}