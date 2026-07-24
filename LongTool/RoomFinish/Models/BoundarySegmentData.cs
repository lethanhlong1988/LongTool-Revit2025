using Autodesk.Revit.DB;

namespace LongTool.RoomFinish.Models;

public class BoundarySegmentData
{
    /// <summary>
    /// Đường hình học tạo nên boundary
    /// </summary>
    public Curve Curve { get; init; }


    /// <summary>
    /// Element tạo nên boundary
    /// </summary>
    public Element? Element { get; init; }


    /// <summary>
    /// Id của Element
    /// </summary>
    public ElementId ElementId { get; init; }


    /// <summary>
    /// Category của Element
    /// </summary>
    public Category? Category { get; init; }


    /// <summary>
    /// Face tạo nên boundary
    /// </summary>
    public Face? Face { get; set; }
}