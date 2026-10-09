using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using LongTool.Domain.Interfaces;
using LongTool.Models;
using LongTool.RevitAdapters.Converters;

namespace LongTool.RevitAdapters.Repositories;

internal class RevitDoorRepository : IDoorRepository
{
    private readonly Document _doc;
    private readonly DoorScheduleItemFactory _factory;

    public RevitDoorRepository(Document doc)
    {
        _doc = doc;
        _factory = new DoorScheduleItemFactory();
    }

    public IReadOnlyList<DoorScheduleItem> GetAll()
    {
        var result = new List<DoorScheduleItem>();

        // 1. Cửa trong host document
        result.AddRange(CollectDoorsFrom(_doc));

        // 2. Cửa trong các Revit Link đã load
        foreach (var linkedDoc in GetLoadedLinkedDocuments())
        {
            result.AddRange(CollectDoorsFrom(linkedDoc));
        }

        return result;
    }

    /// <summary>
    /// Thu thập Door từ một Document (host hoặc link).
    /// </summary>
    private IEnumerable<DoorScheduleItem> CollectDoorsFrom(Document doc)
    {
        return new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_Doors)
            .WhereElementIsNotElementType()
            .OfType<FamilyInstance>()
            .Where(d => d.Symbol != null)
            .Select(_factory.Create);
    }

    /// <summary>
    /// Trả về tất cả linked Document đã load thành công.
    /// </summary>
    private IEnumerable<Document> GetLoadedLinkedDocuments()
    {
        var linkInstances = new FilteredElementCollector(_doc)
            .OfClass(typeof(RevitLinkInstance))
            .Cast<RevitLinkInstance>();

        foreach (var link in linkInstances)
        {
            Document linkedDoc = null;
            try
            {
                linkedDoc = link.GetLinkDocument();
            }
            catch
            {
                // Bỏ qua link lỗi
            }

            if (linkedDoc != null)
                yield return linkedDoc;
        }
    }
}