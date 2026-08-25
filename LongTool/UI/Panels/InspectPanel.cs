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

        // Management Panel Button Show Inspect Element
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnShowInspect", "Show Inspect",
            "LongTool.Inspect.Commands.ShowInspectCommand",
           "Nhấn để hiển thị bảng dữ liệu Element",
            $"{iconPath}Large/43-32.png",
            $"{iconPath}Small/43-16.png");

        // Management Panel Button Show Inspect Element
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnShowProperties", "Show Properties",
            " LongTool.Properties.Commands.ShowPropertiesCommand",
           "Nhấn để hiển thị bảng dữ liệu Element",
            $"{iconPath}Large/41-32.png",
            $"{iconPath}Small/41-16.png");

        // Management Panel Button Test
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnInspect", "Inspect",
            "LongTool.Inspect.Commands.InspectElementCommand",
           "Nhấn để kiểm tra dữ liệu Element",
            $"{iconPath}Large/32-32.png",
            $"{iconPath}Small/32-16.png");

    }
}