using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace LongTool.FinishEngine.Domain;

/// <summary>
/// Đại diện cho một phòng trong Finish Engine.
/// Chỉ chứa dữ liệu nghiệp vụ, không chứa logic xử lý.
/// </summary>
public class FinishRoom
{
    /// <summary>
    /// Id của Room trong Revit.
    /// </summary>
    public ElementId Id { get; set; }

    /// <summary>
    /// Số phòng.
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Tên phòng.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Id của Level chứa phòng.
    /// </summary>
    public ElementId LevelId { get; set; } = ElementId.InvalidElementId;

    /// <summary>
    /// Biên dạng của phòng.
    /// </summary>
    public FinishBoundary? Boundary { get; set; }

    /// <summary>
    /// Danh sách các bề mặt hoàn thiện thuộc phòng.
    /// </summary>
    public List<FinishSurface> Surfaces { get; } = new();
}