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
        // Management Panel Button Test
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnTest03", "Test03",
            //"LongTool.Commands.Test.TestPickPointCircleCommand",
            "LongTool.Commands.Test.TestDimBeamSectionCommand",
            "Nhấn vào nút Test03",
            $"{iconPath}Large/28-32.png",
            $"{iconPath}Small/28-16.png");
        // Management Panel Button Test
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnTest04", "Test04",
            //"LongTool.Commands.Test.TestPickPointCircleCommand",
            "LongTool.Commands.Test.TestDoorBoardCommand",
            "Nhấn vào nút Test04",
            $"{iconPath}Large/29-32.png",
            $"{iconPath}Small/29-16.png");// Management Panel Button Test
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnTest05", "Test05",
            //"LongTool.Commands.Test.TestPickPointCircleCommand",
            "LongTool.Commands.Test.TestFormDialogCommand",
            "Nhấn vào nút Test05",
            $"{iconPath}Large/30-32.png",
            $"{iconPath}Small/30-16.png");

    }
}