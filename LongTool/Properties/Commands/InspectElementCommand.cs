using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LongTool.Properties.Dockable;
using LongTool.Storage.Collectors;

namespace LongTool.Properties.Commands;


[Transaction(TransactionMode.Manual)]
public class InspectElementCommand
    : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        UIDocument uidoc =
            commandData.Application.ActiveUIDocument;


        Document doc =
            uidoc.Document;


        Reference reference =
            uidoc.Selection.PickObject(
                Autodesk.Revit.UI.Selection.ObjectType.Element,
                "Select Element");


        ElementId id =
            reference.ElementId;


        var data =
            ElementCollector.Collect(
                doc,
                id);


        PropertiesDockableProvider.Instance
            ?.SetElementData(
                doc,
                data);


        return Result.Succeeded;
    }
}