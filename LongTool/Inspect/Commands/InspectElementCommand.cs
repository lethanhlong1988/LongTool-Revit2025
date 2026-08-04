using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using LongTool.Inspect.Services;
using LongTool.Storage.Services;
using System;

namespace LongTool.Inspect.Commands;

[Transaction(TransactionMode.Manual)]
public class InspectElementCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        try
        {
            UIDocument uidoc =
                commandData.Application.ActiveUIDocument;

            Document doc =
                uidoc.Document;


            Reference reference;

            try
            {
                reference =
                    uidoc.Selection.PickObject(
                        ObjectType.Element,
                        "Select an element to inspect");
            }
            catch
            {
                return Result.Cancelled;
            }


            ElementStorageService storage = new();

            var data =
                storage.Get(
                    doc,
                    reference.ElementId);


            InspectorService inspector = new();

            var result =
                inspector.Inspect(data);

            string messageText = string.Empty;


            foreach (var group in result)
            {
                messageText +=
                    $"{group.Name}\n";

                messageText +=
                    "----------------\n";


                foreach (var parameter in group.Parameters)
                {
                    messageText +=
                        $"{parameter.Name}: {parameter.DisplayValue}\n";
                }


                messageText += "\n";
            }

            TaskDialog.Show(
                "LongTool Inspect",
                messageText);


            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            TaskDialog.Show(
                "LongTool Error",
                ex.ToString());

            return Result.Failed;
        }
    }
}