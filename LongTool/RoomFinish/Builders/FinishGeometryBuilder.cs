using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;

using LongTool.RoomFinish.Models;

namespace LongTool.RoomFinish.Builders;

public class FinishGeometryBuilder
{
    public List<CurveLoop> BuildProfile(
        FinishFaceData faceData)
    {
        return faceData.Face
            .GetEdgesAsCurveLoops()
            .ToList();
    }
}