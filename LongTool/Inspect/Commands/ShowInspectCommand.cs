using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using LongTool.Inspect.Dockable;

namespace LongTool.Inspect.Commands;

[Transaction(TransactionMode.Manual)]
public class ShowInspectCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        Autodesk.Revit.DB.ElementSet elements)
    {
        var pane = commandData.Application
            .GetDockablePane(new DockablePaneId(InspectPaneIds.Guid));

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