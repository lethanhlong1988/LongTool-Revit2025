using System.Collections.Generic;
using Autodesk.Revit.DB;
using LongTool.Models;
using System;
using LongTool.Services.DoorBoard;   // DoorBoardDrawingService cũ

namespace LongTool.RevitAdapters.Writers;

internal class RevitLegendWriter
{
    private readonly Document _doc;
    private readonly DoorBoardDrawingService _drawingService;

    public RevitLegendWriter(Document doc)
    {
        _doc = doc;
        _drawingService = new DoorBoardDrawingService(doc);
    }

    public IReadOnlyList<string> CreateDoorLegends(
        View templateView,
        IReadOnlyList<DoorScheduleItem> doors,
        XYZ origin)
    {
        var textType = _drawingService.FindFirstTextNoteType();
        if (textType == null)
            throw new InvalidOperationException("Không tìm thấy TextNoteType.");

        var createdNames = new List<string>();

        using var tx = new Transaction(_doc, "Create Door Legends");
        tx.Start();

        foreach (var door in doors)
        {
            var newView = _drawingService.DuplicateLegendAndDraw(
                templateView, door, origin, textType);
            createdNames.Add(newView.Name);
        }

        tx.Commit();
        return createdNames;
    }
}