using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace LongTool.RoomFinish.Models;

public class RoomData
{
    /// <summary>
    /// Room Id
    /// </summary>
    public ElementId Id { get; init; } = ElementId.InvalidElementId;


    /// <summary>
    /// Room Number
    /// </summary>
    public string Number { get; init; } = string.Empty;


    /// <summary>
    /// Room Name
    /// </summary>
    public string Name { get; init; } = string.Empty;


    /// <summary>
    /// Room Area (Internal Unit)
    /// </summary>
    public double Area { get; init; }


    /// <summary>
    /// Level Id
    /// </summary>
    public ElementId LevelId { get; init; } = ElementId.InvalidElementId;


    /// <summary>
    /// Level Name
    /// </summary>
    public string LevelName { get; init; } = string.Empty;


    /// <summary>
    /// Boundary của Room
    /// </summary>
    public List<CurveLoop> BoundaryLoops { get; } = [];


    /// <summary>
    /// Các mặt hoàn thiện bao quanh Room
    /// </summary>
    public List<FinishFaceData> FinishFaces { get; } = [];

    /// <summary>
    /// Các Element hoàn thiện bao quanh Room
    /// </summary>
    public List<FinishElementData> FinishElements { get; } = [];

    /// <summary>
    /// Generated Finish Solids
    /// </summary>
    public List<FinishSolidData> FinishSolids { get; } = [];
}