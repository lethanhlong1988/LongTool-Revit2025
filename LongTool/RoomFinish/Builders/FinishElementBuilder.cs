using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;
using LongTool.RoomFinish.Models;


namespace LongTool.RoomFinish.Builders;

public static class FinishElementBuilder
{
    public static void Build(RoomData roomData)
    {
        roomData.FinishElements.Clear();


        IEnumerable<IGrouping<ElementId, FinishFaceData>> groups =
            roomData.FinishFaces
                    .GroupBy(x => x.HostElement.Id);



        foreach (IGrouping<ElementId, FinishFaceData> group in groups)
        {
            FinishFaceData firstFace =
                group.First();


            FinishElementData finishElement =
                new FinishElementData
                {
                    HostElement = firstFace.HostElement
                };


            foreach (FinishFaceData face in group)
            {
                finishElement.Faces.Add(face);
            }


            roomData.FinishElements.Add(finishElement);
        }
    }
}