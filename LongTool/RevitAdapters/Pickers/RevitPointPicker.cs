using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.Core.Selection;   // namespace cũ của PointPicker, giữ nguyên

namespace LongTool.RevitAdapters.Pickers;

internal class RevitPointPicker
{
    private readonly UIDocument _uidoc;

    public RevitPointPicker(UIDocument uidoc) => _uidoc = uidoc;

    public XYZ? Pick(string prompt)
    {
        var pick = PointPicker.PickPoint(_uidoc, prompt);
        return pick?.Point;
    }
}