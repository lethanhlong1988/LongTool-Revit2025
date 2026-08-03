using Autodesk.Revit.DB;
using LongTool.Storage.Models;
using LongTool.Units;

namespace LongTool.Storage.Services;

public static class ParameterConverter
{
    public static ParameterData? Convert(
        Parameter parameter)
    {
        if (parameter.StorageType == StorageType.None)
        {
            return null;
        }

        var data = new ParameterData
        {
            Name = parameter.Definition.Name
        };


        switch (parameter.StorageType)
        {
            case StorageType.String:

                data.DataType = "Text";

                data.TextValue =
                    parameter.AsString()
                    ?? string.Empty;

                data.DisplayValue =
                    data.TextValue;

                break;


            case StorageType.Integer:

                data.DataType = "Integer";

                int integerValue =
                    parameter.AsInteger();

                data.NumericValue =
                    integerValue;

                data.DisplayValue =
                    integerValue.ToString();

                break;


            case StorageType.ElementId:

                data.DataType = "ElementId";

                data.TextValue =
                    parameter.AsElementId()
                    .Value
                    .ToString();

                data.DisplayValue =
                    data.TextValue;

                break;


            case StorageType.Double:

                double value =
                    parameter.AsDouble();


                ForgeTypeId unitId =
                    parameter.GetUnitTypeId();


                UnitType unitType =
                    RevitUnitMapper.GetUnitType(unitId);


                double convertedValue =
                    UnitConverter.ConvertFromInternal(
                        value,
                        unitType);


                data.DataType =
                    unitType.ToString();


                data.NumericValue =
                    convertedValue;


                data.Unit =
                    unitType switch
                    {
                        UnitType.Length => "mm",
                        UnitType.Area => "m²",
                        UnitType.Volume => "m³",
                        UnitType.Angle => "degree",
                        _ => string.Empty
                    };


                data.DisplayValue =
                    $"{convertedValue} {data.Unit}";


                break;
        }


        return data;
    }
}