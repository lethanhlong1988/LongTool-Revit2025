using Autodesk.Revit.DB;

namespace LongTool.RevitAdapters.Converters;

/// <summary>
/// Chuyển đổi đơn vị giữa Revit internal units và đơn vị hiển thị.
/// </summary>
internal static class RevitUnitConverter
{
    public static double ToMillimeters(double internalValue)
    {
        return UnitUtils.ConvertFromInternalUnits(
            internalValue,
            UnitTypeId.Millimeters);
    }
}