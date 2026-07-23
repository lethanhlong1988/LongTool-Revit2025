using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.RoomFinish.Models;
using LongTool.RoomFinish.Renderers;
using LongTool.RoomFinish.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
            Document doc =
                commandData.Application
                    .ActiveUIDocument
                    .Document;

            // ============================
            // 1. Build Room Data
            // ============================

            RoomFinishService roomFinishService =
                new RoomFinishService(doc);

            List<RoomData> rooms =
                roomFinishService.Build();

            // ============================
            // 2. Render Room Boundary
            // ============================

            if (rooms.Count > 0)
            {
                using Transaction transaction =
                    new Transaction(
                        doc,
                        "Draw Room Boundary");

                transaction.Start();

                RoomBoundaryRenderer renderer =
                    new RoomBoundaryRenderer(
                        doc,
                        doc.ActiveView);

                renderer.Draw(rooms);

                transaction.Commit();
            }

            // ============================
            // 3. Debug Report
            // ============================

            RoomDebugService debugService =
                new RoomDebugService();

            debugService.PrintReport(rooms);

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("========== EXCEPTION ==========");
            Debug.WriteLine(ex.ToString());
            Debug.WriteLine("===============================");

            TaskDialog.Show(
                "Exception",
                ex.ToString());

            message = ex.Message;

            return Result.Failed;
        }
    }
}