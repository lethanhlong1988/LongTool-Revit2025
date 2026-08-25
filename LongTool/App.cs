using Autodesk.Revit.UI;
using LongTool.Inspect.Services;
using LongTool.Properties.Services;
using LongTool.UI.Ribbon;
using System;

namespace LongTool;

public class App : IExternalApplication
{

    //Long version 03
    public Result OnStartup(UIControlledApplication application)
    {
        try
        {
            var ribbonService = new RibbonService(application);
            ribbonService.CreateRibbon();

            var inspectService = new InspectService();
            inspectService.Register(application);

            var propertiesService = new PropertiesService();
            propertiesService.Register(application);

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Lỗi khởi tạo LongTool", ex.Message);
            return Result.Failed;
        }
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        return Result.Succeeded;
    }
}