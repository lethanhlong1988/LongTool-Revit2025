using Autodesk.Revit.UI;
using LongTool.Inspect.Dockable;

namespace LongTool.Inspect.Services;

public class InspectService
{
    public void Register(UIControlledApplication application)
    {
        var provider = new InspectDockableProvider();

        application.RegisterDockablePane(
            new DockablePaneId(InspectPaneIds.Guid),
            "Inspect",
            provider);
    }
}