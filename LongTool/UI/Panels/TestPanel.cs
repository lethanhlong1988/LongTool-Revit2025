using System.Collections.Generic;
using Autodesk.Revit.UI;
using LongTool.UI.Panels;

namespace LongTool.UI.Panels;

public class TestPanel
{
    public void Build(RibbonPanel panel, string assemblyPath)
    {
        var buttonBuilder = new Buttons.ButtonBuilder(assemblyPath);
        string iconPath = "Resources/Icons/";

        // Management Panel Button Test
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnTest", "Test",
            //"LongTool.Commands.Test.TestPickPointCircleCommand",
            "LongTool.Commands.JohAbroad.DrawTableBoardCommand",
            "Nhấn vào nút Test",
            $"{iconPath}Large/26-32.png",
            $"{iconPath}Small/26-16.png");
        // Management Panel Button Test
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnTest02", "Test02",
            //"LongTool.Commands.Test.TestPickPointCircleCommand",
            "LongTool.Commands.JohAbroad.BeamSectionDimensionCommand",
            "Nhấn vào nút Test02",
            $"{iconPath}Large/27-32.png",
            $"{iconPath}Small/27-16.png");

    }
}