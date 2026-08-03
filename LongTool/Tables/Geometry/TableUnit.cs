using Autodesk.Revit.DB;

namespace LongTool.Tables.Geometry;

public static class TableUnit
{
    public static double MmToFeet(double mm)
    {
        return UnitUtils.ConvertToInternalUnits(
            mm,
            UnitTypeId.Millimeters);
    }


    public static double FeetToMm(double feet)
    {
        return UnitUtils.ConvertFromInternalUnits(
            feet,
            UnitTypeId.Millimeters);
    }
}