using System.Collections.Generic;
using Autodesk.Revit.UI;
using LongTool.UI.Panels;

namespace LongTool.UI.Panels;

public class StoragePanel
{
    public void Build(RibbonPanel panel, string assemblyPath)
    {
        var buttonBuilder = new Buttons.ButtonBuilder(assemblyPath);
        string iconPath = "Resources/Icons/";

        // Management Panel Button Test
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnExportElement", "Nut Export Element",
            "LongTool.Storage.Commands.ExportElementCommand",
           "Nhấn để xuất dữ liệu Element ra JSON",
            $"{iconPath}Large/33-32.png",
            $"{iconPath}Small/33-16.png");

    }
}