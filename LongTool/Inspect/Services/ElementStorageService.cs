using Autodesk.Revit.DB;
using LongTool.Storage.Collectors;
using LongTool.Storage.Models;

namespace LongTool.Storage.Services;

public class ElementStorageService
{
    public ElementData Get(
        Document document,
        ElementId elementId)
    {
        return ElementCollector.Collect(document, elementId);
    }
}