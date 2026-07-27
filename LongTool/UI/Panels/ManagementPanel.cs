using System.Collections.Generic;
using Autodesk.Revit.UI;
using LongTool.UI.Panels;

namespace LongTool.UI.Panels;

public class ManagementPanel
{
    public void Build(RibbonPanel panel, string assemblyPath)
    {
        var buttonBuilder = new Buttons.ButtonBuilder(assemblyPath);
        string iconPath = "Resources/Icons/";

        // Management Panel Button 01
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnCreate", "Create",
            "LongTool.Commands.Management.TestLongToolMarkerCommand",
            "Nhấn vào nút 01",
            $"{iconPath}Large/Nut01.png",
            $"{iconPath}Small/Nut01.png");

        // Management Panel Button LongTool Cleaner Command
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnClear", "Cleaner",
            "LongTool.Commands.Management.ClearLongToolObjectsCommand",
            "Nhấn vào nút 02",
            $"{iconPath}Large/Nut01.png",
            $"{iconPath}Small/Nut01.png");

        // Management Panel Button LongTool Find Command
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnFind", "Find",
            "LongTool.Commands.Management.TestLongToolCollectorCommand",
            "Nhấn vào nút 02",
            $"{iconPath}Large/Nut01.png",
            $"{iconPath}Small/Nut01.png");

        // Management Panel Button LongTool Select Command
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnSelect", "Select",
            "LongTool.Commands.Management.SelectLongToolObjectsCommand",
            "Nhấn vào nút 03",
            $"{iconPath}Large/Nut01.png",
            $"{iconPath}Small/Nut01.png");
    }
}