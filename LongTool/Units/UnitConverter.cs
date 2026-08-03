using Autodesk.Revit.DB;

namespace LongTool.Units;

public static class UnitConverter
{
    public static double ConvertFromInternal(
        double value,
        UnitType unitType)
    {
        return unitType switch
        {
            UnitType.Length =>
                UnitUtils.ConvertFromInternalUnits(
                    value,
                    UnitTypeId.Millimeters),

            UnitType.Area =>
                UnitUtils.ConvertFromInternalUnits(
                    value,
                    UnitTypeId.SquareMeters),

            UnitType.Volume =>
                UnitUtils.ConvertFromInternalUnits(
                    value,
                    UnitTypeId.CubicMeters),

            UnitType.Angle =>
                UnitUtils.ConvertFromInternalUnits(
                    value,
                    UnitTypeId.Degrees),

            _ => value
        };
    }
}