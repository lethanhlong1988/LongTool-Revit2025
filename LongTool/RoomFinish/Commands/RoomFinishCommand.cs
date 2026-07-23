using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using LongTool.RoomFinish.Models;
using LongTool.RoomFinish.Renderers;
using LongTool.RoomFinish.Services;


namespace LongTool.RoomFinish.Commands;


[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class RoomFinishCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        TaskDialog.Show(
            "Room Finish",
            "Command Started");


        try
        {
            Document document =
                commandData.Application.ActiveUIDocument.Document;


            // ============================
            // 1. Collect Rooms
            // ============================

            RoomCollector roomCollector =
                new RoomCollector(document);

            List<RoomData> rooms =
                roomCollector.Collect();



            // ============================
            // 2. Build Room Boundary
            // ============================

            RoomBoundaryService boundaryService =
                new RoomBoundaryService(document);


            foreach (RoomData room in rooms)
            {
                boundaryService.Build(room);
            }

            // ============================
            // 2.5 Analyze Room Spatial
            // ============================

            RoomSpatialService spatialService =
                new RoomSpatialService(document);


            if (rooms.Count > 0)
            {
                spatialService.Build(rooms[0]);
            }

            // ============================
            // DEBUG FINISH FACES
            // ============================

            if (rooms.Count > 0)
            {
                RoomData room = rooms[0];


                Debug.WriteLine("==============================");
                Debug.WriteLine(
                    $"Room : {room.Number}");

                Debug.WriteLine(
                    $"Finish Face Count : {room.FinishFaces.Count}");

                Debug.WriteLine("==============================");


                foreach (FinishFaceData faceData in room.FinishFaces)
                {
                    Debug.WriteLine("------------------------------");

                    Debug.WriteLine(
                        $"Element Id : {faceData.HostElement.Id}");

                    Debug.WriteLine(
                        $"Category : {faceData.HostElement.Category?.Name}");

                    Debug.WriteLine(
                        $"Area : {faceData.Area}");
                }
            }



            // ============================
            // 3. Render Room Boundary
            // ============================

            if (rooms.Count > 0)
            {
                using (Transaction transaction =
                       new Transaction(
                           document,
                           "Draw Room Boundary"))
                {
                    transaction.Start();


                    RoomBoundaryRenderer renderer =
                        new RoomBoundaryRenderer(
                            document,
                            document.ActiveView);


                    foreach (RoomData room in rooms)
                    {
                        renderer.Draw(room);
                    }


                    transaction.Commit();
                }
            }



            // ============================
            // DEBUG ROOM BOUNDARY
            // ============================

            Debug.WriteLine("==============================");
            Debug.WriteLine($"Total Rooms : {rooms.Count}");
            Debug.WriteLine("==============================");


            if (rooms.Count > 0)
            {
                RoomData room = rooms[0];


                Debug.WriteLine(
                    $"Room Number : {room.Number}");

                Debug.WriteLine(
                    $"Room Name   : {room.Name}");

                Debug.WriteLine(
                    $"Area        : {room.Area}");

                Debug.WriteLine(
                    $"Loop Count  : {room.BoundaryLoops.Count}");



                for (int i = 0;
                     i < room.BoundaryLoops.Count;
                     i++)
                {
                    CurveLoop loop =
                        room.BoundaryLoops[i];


                    Debug.WriteLine("------------------------------");

                    Debug.WriteLine(
                        $"Loop Index : {i}");

                    Debug.WriteLine(
                        $"Is Open : {loop.IsOpen()}");


                    Debug.WriteLine(
                        $"Curve Count : {loop.Count()}");


                    foreach (Curve curve in loop)
                    {
                        Debug.WriteLine(
                            $"Curve Type : {curve.GetType().Name}");

                        Debug.WriteLine(
                            $"Length : {curve.Length}");
                    }
                }
            }


            Debug.WriteLine("==============================");
            Debug.WriteLine("DEBUG FINISHED");
            Debug.WriteLine("==============================");


            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.Message;

            return Result.Failed;
        }
    }
}