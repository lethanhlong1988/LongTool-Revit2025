using System.Collections.Generic;
using Autodesk.Revit.UI;
using LongTool.UI.Panels;

namespace LongTool.UI.Panels;

public class JobAbroadPanel
{
    public void Build(RibbonPanel panel, string assemblyPath)
    {
        var buttonBuilder = new Buttons.ButtonBuilder(assemblyPath);
        string iconPath = "Resources/Icons/";

        // Management Panel Button HideLevelsHead
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnHideLevelsHeadCommand", "HideLevelsHead",
            //"LongTool.Commands.Test.TestPickPointCircleCommand",
            "LongTool.Commands.JohAbroad.HideLevelsHeadCommand",
            "Nhấn vào nút HideLevelsHead",
            $"{iconPath}Large/26-32.png",
            $"{iconPath}Small/26-16.png");

        // Management Panel Button ExpandCropTopToLevelCommand
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnExpandCropTopToLevelCommand", "ExpandCropTopToLevel",
            //"LongTool.Commands.Test.TestPickPointCircleCommand",
            "LongTool.Commands.JohAbroad.ExpandCropTopToLevelCommand",
            "Nhấn vào nút ExpandCropTopToLevel",
            $"{iconPath}Large/28-32.png",
            $"{iconPath}Small/28-16.png");

        // Management Panel Button CropBottomToLevelCommand
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnCropBottomToLevelCommand", "CropBottomToLevel",
            //"LongTool.Commands.Test.TestPickPointCircleCommand",
            "LongTool.Commands.JohAbroad.CropBottomToLevelCommand",
            "Nhấn vào nút CropBottomToLevelCommand",
            $"{iconPath}Large/29-32.png",
            $"{iconPath}Small/29-16.png");

        // Management Panel Button ExtendGridTopToViewCommand
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnAdjustGridTopToCropTopCommand", "AdjustGridTopToCropTopCommand",
            //"LongTool.Commands.Test.TestPickPointCircleCommand",
            "LongTool.Commands.JohAbroad.AdjustGridTopToCropTopCommand",
            "Nhấn vào nút AdjustGridTopToCropTop",
            $"{iconPath}Large/30-32.png",
            $"{iconPath}Small/30-16.png");

        // Management Panel Button DrawWallCenterLineCommand
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnDrawWallCenterLineCommand", "DrawWallCenterLine",
            //"LongTool.Commands.Test.TestPickPointCircleCommand",
            "LongTool.Commands.JohAbroad.DrawWallCenterLineCommand",
            "Nhấn vào nút DrawWallCenterLine",
            $"{iconPath}Large/31-32.png",
            $"{iconPath}Small/31-16.png");

    }
}