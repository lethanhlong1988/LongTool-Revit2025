using Autodesk.Revit.DB;
using LongTool.Models;

namespace LongTool.RevitAdapters.Converters;

/// <summary>
/// Chuyển FamilyInstance/FamilySymbol (Revit) → DoorScheduleItem (Domain).
/// </summary>
internal class DoorScheduleItemFactory
{
    public DoorScheduleItem Create(FamilyInstance door)
    {
        var symbol = door.Symbol;

        var jName = GetText(symbol, "J_Name");
        var jNumber = GetText(symbol, "J_Number");

        var symbolName =
            !string.IsNullOrWhiteSpace(jName) && !string.IsNullOrWhiteSpace(jNumber)
                ? $"{jName}-{jNumber}"
                : !string.IsNullOrWhiteSpace(jName)
                    ? jName
                    : jNumber;

        return new DoorScheduleItem
        {
            Symbol = symbolName,
            Quantity = 1,
            Type = GetText(symbol, "J_Type"),
            Location = GetText(symbol, "J_Location"),
            Glass = GetText(symbol, "J_Glass"),
            Finish = GetText(symbol, "J_Finish"),
            Hardware = GetText(symbol, "J_Hardware"),
            Remarks = GetText(symbol, "J_Remarks"),
            DoorThickness = GetText(symbol, "J_DoorThickness"),
            FrameThickness = GetText(symbol, "J_FrameThickness"),
            ShapeWidth = GetLengthMm(symbol, "Width"),
            ShapeHeight = GetLengthMm(symbol, "Height")
        };
    }

    private static string GetText(Element element, string name)
    {
        return element.LookupParameter(name)?.AsString() ?? string.Empty;
    }

    private static double GetLengthMm(Element element, string name)
    {
        var p = element.LookupParameter(name);
        if (p == null || p.StorageType != StorageType.Double)
            return 0;
        return RevitUnitConverter.ToMillimeters(p.AsDouble());
    }
}