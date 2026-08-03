using Autodesk.Revit.DB;

namespace LongTool.Units;

public static class RevitUnitMapper
{
    public static UnitType GetUnitType(
        ForgeTypeId unitId)
    {
        if (unitId == UnitTypeId.Millimeters ||
            unitId == UnitTypeId.Meters ||
            unitId == UnitTypeId.Feet)
        {
            return UnitType.Length;
        }


        if (unitId == UnitTypeId.SquareMeters ||
            unitId == UnitTypeId.SquareFeet)
        {
            return UnitType.Area;
        }


        if (unitId == UnitTypeId.CubicMeters ||
            unitId == UnitTypeId.CubicFeet)
        {
            return UnitType.Volume;
        }


        if (unitId == UnitTypeId.Degrees ||
            unitId == UnitTypeId.Radians)
        {
            return UnitType.Angle;
        }


        return UnitType.None;
    }
}