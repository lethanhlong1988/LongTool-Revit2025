using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace LongTool.RoomFinish.Models;

public class FinishElementData
{
    /// <summary>
    /// Element tạo nên bề mặt hoàn thiện
    /// </summary>
    public Element HostElement { get; init; }


    /// <summary>
    /// Các Face thuộc Element này
    /// </summary>
    public List<FinishFaceData> Faces { get; } = [];


    /// <summary>
    /// Tổng diện tích hoàn thiện
    /// </summary>
    public double Area
    {
        get
        {
            double total = 0;

            foreach (FinishFaceData face in Faces)
            {
                total += face.Area;
            }

            return total;
        }
    }
}