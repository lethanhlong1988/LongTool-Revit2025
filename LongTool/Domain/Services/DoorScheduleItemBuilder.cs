using LongTool.Domain.Interfaces;
using LongTool.Models;

namespace LongTool.Domain.Services;

/// <summary>
/// Build DoorScheduleItem từ IParameterReader.
/// Logic thuần — không phụ thuộc Revit.
/// </summary>
public class DoorScheduleItemBuilder
{
    public DoorScheduleItem Build(IParameterReader reader, int quantity = 1)
    {
        var jName = reader.GetText("J_Name");
        var jNumber = reader.GetText("J_Number");

        var symbolName = BuildSymbolName(jName, jNumber);

        return new DoorScheduleItem
        {
            Symbol = symbolName,
            Quantity = quantity,

            Type = reader.GetText("J_Type"),
            Location = reader.GetText("J_Location"),
            Glass = reader.GetText("J_Glass"),
            Finish = reader.GetText("J_Finish"),
            Hardware = reader.GetText("J_Hardware"),
            Remarks = reader.GetText("J_Remarks"),
            DoorThickness = reader.GetText("J_DoorThickness"),
            FrameThickness = reader.GetText("J_FrameThickness"),

            ShapeWidth = reader.GetLengthInMillimeters("Width"),
            ShapeHeight = reader.GetLengthInMillimeters("Height")
        };
    }

    private static string BuildSymbolName(string jName, string jNumber)
    {
        var hasName = !string.IsNullOrWhiteSpace(jName);
        var hasNumber = !string.IsNullOrWhiteSpace(jNumber);

        if (hasName && hasNumber)
            return $"{jName}-{jNumber}";

        if (hasName)
            return jName;

        return jNumber;
    }
}