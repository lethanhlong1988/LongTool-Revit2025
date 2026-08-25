using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using LongTool.Properties.Dockable;


namespace LongTool.Properties.Commands;


[Transaction(TransactionMode.Manual)]
public class ShowPropertiesCommand
    : IExternalCommand
{

    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        Autodesk.Revit.DB.ElementSet elements)
    {

        var pane =
            commandData.Application
            .GetDockablePane(
                new DockablePaneId(
                    PropertiesPaneIds.Guid));


        if (pane.IsShown())
        {
            pane.Hide();
        }
        else
        {
            pane.Show();
        }


        return Result.Succeeded;
    }
}