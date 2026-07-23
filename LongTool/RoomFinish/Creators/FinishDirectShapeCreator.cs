using Autodesk.Revit.DB;

using LongTool.RoomFinish.Models;

namespace LongTool.RoomFinish.Creators;

public class FinishDirectShapeCreator
{
    private readonly Document _document;

    public FinishDirectShapeCreator(
        Document document)
    {
        _document = document;
    }

    public void Create(
        FinishFaceData faceData)
    {

    }
}