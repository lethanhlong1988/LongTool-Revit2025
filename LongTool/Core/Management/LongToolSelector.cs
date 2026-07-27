using System;
using System.Collections.Generic;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace LongTool.Core.Management;

public static class LongToolSelector
{
    public static int SelectAll(UIDocument uiDocument)
    {
        if (uiDocument is null)
        {
            throw new ArgumentNullException(nameof(uiDocument));
        }

        Document document = uiDocument.Document;

        ICollection<ElementId> elementIds = LongToolElementCollector.Collect(document);

        uiDocument.Selection.SetElementIds(elementIds);

        return elementIds.Count;
    }
}