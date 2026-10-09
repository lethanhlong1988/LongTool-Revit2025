using Autodesk.Revit.DB;
using LongTool.Domain.Interfaces;

namespace LongTool.RevitAdapters.Converters;

/// <summary>
/// Implement IParameterReader cho một Element của Revit.
/// Đọc text parameter và length parameter (convert sang mm).
/// </summary>
internal class FamilySymbolParameterReader : IParameterReader
{
    private readonly Element _element;

    public FamilySymbolParameterReader(Element element)
    {
        _element = element;
    }

    public string GetText(string parameterName)
    {
        var p = _element.LookupParameter(parameterName);
        return p?.AsString() ?? string.Empty;
    }

    public double GetLengthInMillimeters(string parameterName)
    {
        var p = _element.LookupParameter(parameterName);

        if (p == null || p.StorageType != StorageType.Double)
            return 0;

        return UnitUtils.ConvertFromInternalUnits(
            p.AsDouble(),
            UnitTypeId.Millimeters);
    }
}