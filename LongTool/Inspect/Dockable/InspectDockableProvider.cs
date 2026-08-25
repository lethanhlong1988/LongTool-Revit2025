using Autodesk.Revit.UI;
using LongTool.Inspect.Views;

namespace LongTool.Inspect.Dockable;

public class InspectDockableProvider : IDockablePaneProvider
{
    public void SetupDockablePane(DockablePaneProviderData data)
    {
        data.FrameworkElement = new InspectView();

        data.InitialState = new DockablePaneState
        {
            DockPosition = DockPosition.Right
        };
    }
}