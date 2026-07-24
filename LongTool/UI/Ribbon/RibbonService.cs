using System;
using Autodesk.Revit.UI;

namespace LongTool.UI.Ribbon;

public class RibbonService
{
    private readonly UIControlledApplication _application;
    private readonly string _assemblyPath;

    public RibbonService(UIControlledApplication application)
    {
        _application = application;
        _assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
    }

    public void CreateRibbon()
    {
        try
        {
            var ribbonBuilder = new RibbonBuilder(_application);
            ribbonBuilder.Build();
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Lỗi tạo Ribbon", ex.Message);
            throw;
        }
    }
}