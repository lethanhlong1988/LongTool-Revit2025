using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace LongTool.Infrastructure;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public abstract class CommandBase : IExternalCommand
{
    public abstract Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements);

    protected void ShowMessage(string title, string content)
    {
        TaskDialog.Show(title, content);
    }

    protected UIApplication GetUIApplication(ExternalCommandData commandData)
    {
        return commandData.Application;
    }

    protected UIDocument GetUIDocument(ExternalCommandData commandData)
    {
        return commandData.Application.ActiveUIDocument;
    }

    protected Document GetDocument(ExternalCommandData commandData)
    {
        return commandData.Application.ActiveUIDocument.Document;
    }
}