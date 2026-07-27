using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace LongTool.Utils
{
    public static class SelectionUtils
    {
        public static Element GetSelectedElement(UIDocument uiDoc)
        {
            var ids = uiDoc.Selection.GetElementIds();

            if (ids.Count == 0)
                return null;

            return uiDoc.Document.GetElement(ids.First());
        }
    }
}