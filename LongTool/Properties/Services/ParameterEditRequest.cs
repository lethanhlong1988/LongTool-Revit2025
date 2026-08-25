using Autodesk.Revit.DB;

namespace LongTool.Properties.Services;

public class ParameterEditRequest
{
    public Document Document { get; }

    public long ElementId { get; }

    public string ParameterName { get; }

    public string Value { get; }

    public ParameterEditRequest(
        Document document,
        long elementId,
        string parameterName,
        string value)
    {
        Document = document;
        ElementId = elementId;
        ParameterName = parameterName;
        Value = value;
    }
}