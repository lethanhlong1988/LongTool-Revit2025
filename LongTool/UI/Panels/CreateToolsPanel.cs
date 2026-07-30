using System.Collections.Generic;
using Autodesk.Revit.UI;

namespace LongTool.UI.Panels;

public class CreateToolsPanel
{
    public void Build(RibbonPanel panel, string assemblyPath)
    {
        var buttonBuilder = new Buttons.ButtonBuilder(assemblyPath);
        string iconPath = "Resources/Icons/";

        // Nút 01
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnNut01", "Nut 01",
            "LongTool.Commands.Long.FinishEngineTestCommand",
            "Nhấn vào nút 01",
            $"{iconPath}Large/30-32.png",
            $"{iconPath}Small/30-16.png");

        // Nút 02 - SỬA COMMAND
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnRoomFinish", "RoomFinish",
            "LongTool.RoomFinish.Commands.RoomFinishCommand",  
            "Lệnh thống kê Rooms",
            $"{iconPath}Large/31-32.png",
            $"{iconPath}Small/31-16.png");

        panel.AddSeparator();

        // Stacked buttons
        buttonBuilder.AddTwoStackedButtonsToPanel(panel,
            ("btnDoor", "Cửa", "LongTool.Commands.Basic.CommandCreateDoor",
                "Tạo cửa",
                $"{iconPath}Large/32-32.png", $"{iconPath}Small/32-16.png"),
            ("btnWindow", "Cửa sổ", "LongTool.Commands.Basic.CommandCreateWindow",
                "Tạo cửa sổ",
                $"{iconPath}Large/33-32.png", $"{iconPath}Small/33-16.png")
        );
    }
}