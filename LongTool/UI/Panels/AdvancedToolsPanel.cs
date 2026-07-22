using System.Collections.Generic;
using Autodesk.Revit.UI;

namespace LongTool.UI.Panels
{
    public class AdvancedToolsPanel
    {
        public void Build(RibbonPanel panel, string assemblyPath)
        {
            var buttonBuilder = new Buttons.ButtonBuilder(assemblyPath);
            string iconPath = "Resources/Icons/";

            // Trong AdvancedToolsPanel.cs, thay vì RadioGroup, dùng SplitButton:
            var radioOptions = new List<(string id, string text, string commandPath, string tooltip, string largeIcon, string smallIcon)>
            {
                ("radio1", "Option 1", "LongTool.Commands.Basic.CommandHello", "Tùy chọn 1", null, null),
                ("radio2", "Option 2", "LongTool.Commands.Basic.CommandInfo", "Tùy chọn 2", null, null),
                ("radio3", "Option 3", "LongTool.Commands.Basic.CommandSetting", "Tùy chọn 3", null, null)
            };
            buttonBuilder.CreateSplitButton(panel, "radioGroup", "Tùy chọn", radioOptions, "Chọn tùy chọn");

        }
    }
}