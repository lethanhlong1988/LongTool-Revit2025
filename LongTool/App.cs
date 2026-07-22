using System;
using Autodesk.Revit.UI;
using LongTool.UI.Ribbon;

namespace LongTool
{
    public class App : IExternalApplication
    {
        //Commant kiểm tra thử cái nhé
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                var ribbonService = new RibbonService(application);
                ribbonService.CreateRibbon();
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
}