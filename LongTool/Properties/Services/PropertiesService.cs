using Autodesk.Revit.UI;
using LongTool.Properties.Dockable;

namespace LongTool.Properties.Services;

public class PropertiesService
{
    public void Register(
        UIControlledApplication application)
    {
        var provider =
            new PropertiesDockableProvider();


        application.RegisterDockablePane(
            new DockablePaneId(
                PropertiesPaneIds.Guid),
            "Properties",
            provider);
    }
}