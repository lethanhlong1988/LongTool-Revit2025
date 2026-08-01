using Autodesk.Revit.DB;

namespace LongTool.Core.Selection;

public sealed class PickPointResult
{
    public XYZ Point { get; }

    public View View { get; }


    public ViewType ViewType =>
        View.ViewType;


    public XYZ ViewDirection =>
        View.ViewDirection;


    public XYZ UpDirection =>
        View.UpDirection;



    public PickPointResult(
        XYZ point,
        View view)
    {
        Point = point;
        View = view;
    }
}