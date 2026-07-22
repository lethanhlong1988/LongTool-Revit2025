using System;
using Autodesk.Revit.UI;

namespace LongTool.UI.Ribbon
{
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

                // Tạo Panel 1: Công cụ cơ bản
                var basicPanel = _application.CreateRibbonPanel(RibbonConstants.TabName, RibbonConstants.BasicToolsPanel);
                new Panels.BasicToolsPanel().Build(basicPanel, _assemblyPath);

                // Tạo Panel 2: Công cụ nâng cao
                //var advancedPanel = _application.CreateRibbonPanel(RibbonConstants.TabName, RibbonConstants.AdvancedToolsPanel);
                //new Panels.AdvancedToolsPanel().Build(advancedPanel, _assemblyPath);

                // Panel 3: Công cụ tạo mới (THÊM MỚI)
                var createPanel = _application.CreateRibbonPanel(RibbonConstants.TabName, RibbonConstants.LongToolsPanel);
                new Panels.CreateToolsPanel().Build(createPanel, _assemblyPath);

            }
            catch (Exception ex)
            {
                TaskDialog.Show("Lỗi tạo Ribbon", ex.Message);
                throw;
            }
        }
    }
}