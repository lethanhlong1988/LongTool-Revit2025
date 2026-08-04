using System.Collections.Generic;
using Autodesk.Revit.UI;
using LongTool.UI.Panels;

namespace LongTool.UI.Panels;

public class InspectPanel
{
    public void Build(RibbonPanel panel, string assemblyPath)
    {
        var buttonBuilder = new Buttons.ButtonBuilder(assemblyPath);
        string iconPath = "Resources/Icons/";

        // Management Panel Button Test
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnInspect", "Nut Inspect Element",
            "LongTool.Inspect.Commands.InspectElementCommand",
           "Nhấn để kiểm tra dữ liệu Element",
            $"{iconPath}Large/32-32.png",
            $"{iconPath}Small/32-16.png");

    }
}