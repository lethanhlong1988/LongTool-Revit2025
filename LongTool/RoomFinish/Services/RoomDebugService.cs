using System.Collections.Generic;
using System.Diagnostics;
using LongTool.RoomFinish.Models;

namespace LongTool.RoomFinish.Services
{
    public class RoomDebugService
    {
        public void PrintReport(List<RoomData> rooms)
        {
            if (rooms == null || rooms.Count == 0)
            {
                Debug.WriteLine("==============================");
                Debug.WriteLine("No rooms found in the document");
                Debug.WriteLine("==============================");
                return;
            }

            PrintRoomHeader(rooms);

            foreach (RoomData room in rooms)
            {
                PrintRoomInfo(room);
                Debug.WriteLine("------------------------------");
            }

            Debug.WriteLine("==============================");
            Debug.WriteLine($"Total Rooms Processed: {rooms.Count}");
            Debug.WriteLine("DEBUG FINISHED");
            Debug.WriteLine("==============================");
        }

        private void PrintRoomHeader(List<RoomData> rooms)
        {
            Debug.WriteLine("==============================");
            Debug.WriteLine($"Total Rooms : {rooms.Count}");
            Debug.WriteLine("==============================");
        }

        private void PrintRoomInfo(RoomData room)
        {
            Debug.WriteLine(
                $"Room Number : {room.Number}");

            Debug.WriteLine(
                $"Room Name   : {room.Name}");

            Debug.WriteLine(
                $"Area        : {room.Area}");

            Debug.WriteLine(
                $"Loop Count  : {room.BoundaryLoops?.Count ?? 0}");

            Debug.WriteLine(
                $"Finish Face Count : {room.FinishFaces?.Count ?? 0}");

            Debug.WriteLine(
                $"Finish Element Count : {room.FinishElements?.Count ?? 0}");

            Debug.WriteLine(
                $"Finish Solid Count : {room.FinishSolids?.Count ?? 0}");
        }
    }
}