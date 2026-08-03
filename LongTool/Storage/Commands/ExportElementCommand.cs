using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using LongTool.Storage.Collectors;
using LongTool.Storage.Models;
using LongTool.Storage.Services;
using System;

namespace LongTool.Storage.Commands;

[Transaction(TransactionMode.Manual)]
public class ExportElementCommand : IExternalCommand
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
                        "Select an element to export");
            }
            catch
            {
                return Result.Cancelled;
            }


            ElementData data =
                ElementCollector.Collect(
                    doc,
                    reference.ElementId);


            JsonStorageService.Save(
                doc,
                data);


            TaskDialog.Show(
                "LongTool",
                $"Exported:\n{data.Category}\n{data.Name}");


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