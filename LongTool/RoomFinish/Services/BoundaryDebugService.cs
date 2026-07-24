using System.Collections.Generic;
using System.Diagnostics;
using LongTool.RoomFinish.Models;

namespace LongTool.RoomFinish.Services;

public class BoundaryDebugService
{
    public void Print(
        List<BoundarySegmentData> segments)
    {
        if (segments == null || segments.Count == 0)
        {
            Debug.WriteLine(
                "No boundary segments found");

            return;
        }


        int index = 1;


        foreach (BoundarySegmentData segment in segments)
        {
            Debug.WriteLine(
                $"Segment {index}");


            if (segment.Element != null)
            {
                Debug.WriteLine(
                    $"Element Type : {segment.Element.GetType().Name}");

                Debug.WriteLine(
                    $"Element Name : {segment.Element.Name}");

                Debug.WriteLine(
                    $"Element Id   : {segment.ElementId}");

                Debug.WriteLine(
                    $"Category     : {segment.Category?.Name}");
            }
            else
            {
                Debug.WriteLine(
                    "Element : null");
            }


            Debug.WriteLine("------------------------------");

            index++;
        }
    }
}