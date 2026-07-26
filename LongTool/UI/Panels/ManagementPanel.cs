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
            "btnManagement01", "Management 01",
            "LongTool.Commands.Management.TestLongToolMarkerCommand",
            "Nhấn vào nút 01",
            $"{iconPath}Large/Nut01.png",
            $"{iconPath}Small/Nut01.png");

        // Management Panel Button TestLongToolElementCollectionCommand
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnManagement02", "Nut Test",
            "LongTool.Commands.Management.TestLongToolCollectorCommand",
            "Nhấn vào nút 02",
            $"{iconPath}Large/Nut01.png",
            $"{iconPath}Small/Nut01.png");

    }
}