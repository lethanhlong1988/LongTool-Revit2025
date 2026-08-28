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
            "btnTest", "Nut Test",
            //"LongTool.Commands.Test.TestPickPointCircleCommand",
            "LongTool.Commands.JohAbroad.HideLevelsHeadCommand",
            "Nhấn vào nút Test",
            $"{iconPath}Large/26-32.png",
            $"{iconPath}Small/26-16.png");

    }
}