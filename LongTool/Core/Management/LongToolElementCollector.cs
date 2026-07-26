using Autodesk.Revit.DB;

using LongTool.Core.Storage;
using System.Collections.Generic;
using System;

namespace LongTool.Core.Management;

public static class LongToolElementCollector
{
    public static ICollection<ElementId> Collect(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var elementIds = new List<ElementId>();

        var collector = new FilteredElementCollector(document)
            .WhereElementIsNotElementType();

        foreach (Element element in collector)
        {
            if (LongToolMarker.IsMarked(element))
            {
                elementIds.Add(element.Id);
            }
        }

        return elementIds;
    }
}