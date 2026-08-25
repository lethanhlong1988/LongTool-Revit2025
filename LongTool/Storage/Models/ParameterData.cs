namespace LongTool.Storage.Models;

public class ParameterData
{
    public string Name { get; set; } = string.Empty;

    public bool IsTypeParameter { get; set; }

    public string DataType { get; set; } = string.Empty;

    public double? NumericValue { get; set; }

    public string? TextValue { get; set; }

    public string Unit { get; set; } = string.Empty;

    public string DisplayValue { get; set; } = string.Empty;
}