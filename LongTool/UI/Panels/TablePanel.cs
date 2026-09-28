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
            "btnTableDraw", "Table\nDraw",
            "LongTool.Commands.TableCommand.DrawTableCommand",
            "Nhấn vào Lệnh vẽ bảng",
            $"{iconPath}Large/30-32.png",
            $"{iconPath}Small/30-16.png");

        // Tabel Draw Button
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnTableDraw02", "Table\nDraw02",
            "LongTool.Commands.TableCommand.TestTableRenderCommand",
            "Nhấn vào Lệnh vẽ bảng",
            $"{iconPath}Large/31-32.png",
            $"{iconPath}Small/31-16.png");

    }
}