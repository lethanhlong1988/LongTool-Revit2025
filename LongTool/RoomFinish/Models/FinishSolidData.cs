using Autodesk.Revit.DB;

namespace LongTool.RoomFinish.Models;

public class FinishSolidData
{
    /// <summary>
    /// Host Face
    /// </summary>
    public FinishFaceData FaceData { get; init; } = null!;

    /// <summary>
    /// Generated Solid
    /// </summary>
    public Solid Solid { get; init; } = null!;
}