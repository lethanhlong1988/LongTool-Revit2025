using System.Collections.Generic;
using Autodesk.Revit.UI;

namespace LongTool.UI.Panels
{
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
                $"{iconPath}Large/Nut01.png",
                $"{iconPath}Small/Nut01.png");

            // Nút 02 - SỬA COMMAND
            buttonBuilder.AddPushButtonToPanel(panel,
                "btnRoomFinish", "RoomFinish",
                "LongTool.RoomFinish.Commands.RoomFinishCommand",  
                "Lệnh thống kê Rooms",
                $"{iconPath}Large/Nut02.png",
                $"{iconPath}Small/Nut02.png");

            panel.AddSeparator();

            // Stacked buttons
            buttonBuilder.AddTwoStackedButtonsToPanel(panel,
                ("btnDoor", "Cửa", "LongTool.Commands.Basic.CommandCreateDoor",
                    "Tạo cửa",
                    $"{iconPath}Large/door.png", $"{iconPath}Small/door.png"),
                ("btnWindow", "Cửa sổ", "LongTool.Commands.Basic.CommandCreateWindow",
                    "Tạo cửa sổ",
                    $"{iconPath}Large/window.png", $"{iconPath}Small/window.png")
            );
        }
    }
}