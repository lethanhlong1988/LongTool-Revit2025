using System.Collections.Generic;
using Autodesk.Revit.UI;
using LongTool.UI.Panels;

namespace LongTool.UI.Panels;

public class TabelPanel
{
    public void Build(RibbonPanel panel, string assemblyPath)
    {
        var buttonBuilder = new Buttons.ButtonBuilder(assemblyPath);
        string iconPath = "Resources/Icons/";

        // Tabel Panel
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnTableDraw", "Table Draw",
            "LongTool.Commands.Table.DrawTableCommand",
            "Nhấn vào Lệnh vẽ bảng",
            $"{iconPath}Large/Nut01.png",
            $"{iconPath}Small/Nut01.png");

    }
}