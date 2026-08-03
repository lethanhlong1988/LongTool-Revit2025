using System;
using Autodesk.Revit.UI;

namespace LongTool.UI.Ribbon;

public class RibbonBuilder
{
    private readonly UIControlledApplication _application;
    private readonly string _assemblyPath;

    public RibbonBuilder(UIControlledApplication application)
    {
        _application = application;
        _assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
    }

    public void Build()
    {
        try
        {
            // Tạo Tab
            _application.CreateRibbonTab(RibbonConstants.TabName);

            // Tạo Panel : Công cụ kiểm tra cơ bản
            var TestPanel = _application.CreateRibbonPanel(RibbonConstants.TabName, RibbonConstants.TestPanel);
            new Panels.TestPanel().Build(TestPanel, _assemblyPath);

            // Tạo Panel : Công cụ cơ bản
            //var basicPanel = _application.CreateRibbonPanel(RibbonConstants.TabName, RibbonConstants.BasicToolsPanel);
            //new Panels.BasicToolsPanel().Build(basicPanel, _assemblyPath);

            // Tạo Panel : Công cụ nâng cao
            //var advancedPanel = _application.CreateRibbonPanel(RibbonConstants.TabName, RibbonConstants.AdvancedToolsPanel);
            //new Panels.AdvancedToolsPanel().Build(advancedPanel, _assemblyPath);

            // Panel : Công cụ tạo mới
            var createPanel = _application.CreateRibbonPanel(RibbonConstants.TabName, RibbonConstants.LongToolsPanel);
            new Panels.CreateToolsPanel().Build(createPanel, _assemblyPath);

            // Panel : Công cụ Management
            var managementPanel = _application.CreateRibbonPanel(RibbonConstants.TabName, RibbonConstants.ManagementPanel);
            new Panels.ManagementPanel().Build(managementPanel, _assemblyPath);

            // Panel : Công cụ Tabel
            var TabelPanel = _application.CreateRibbonPanel(RibbonConstants.TabName, RibbonConstants.TabelPanel);
            new Panels.TabelPanel().Build(TabelPanel, _assemblyPath);

            // Panel : Công cụ Storage
            var StoragePanel = _application.CreateRibbonPanel(RibbonConstants.TabName, RibbonConstants.StoragePanel);
            new Panels.StoragePanel().Build(StoragePanel, _assemblyPath);

        }
        catch (Exception ex)
        {
            TaskDialog.Show("Lỗi tạo Ribbon", ex.Message);
            throw;
        }
    }
}