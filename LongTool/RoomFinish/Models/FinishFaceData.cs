using Autodesk.Revit.DB;

namespace LongTool.RoomFinish.Models;

public class FinishFaceData
{
    /// <summary>
    /// Phần tử tạo nên mặt hoàn thiện
    /// </summary>
    public Element HostElement { get; init; }


    /// <summary>
    /// Mặt hình học tiếp xúc với Room
    /// </summary>
    public Face Face { get; init; }


    /// <summary>
    /// Thông tin liên kết Room - Element
    /// </summary>
    public SpatialElementBoundarySubface BoundarySubface { get; init; }


    /// <summary>
    /// Diện tích mặt hoàn thiện
    /// </summary>
    public double Area { get; init; }
}