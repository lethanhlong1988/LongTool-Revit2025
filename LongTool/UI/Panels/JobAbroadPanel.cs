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
        //buttonBuilder.AddPushButtonToPanel(panel,
        //    "btnHideLevelsHeadCommand", "Hide\nLevelsHead",
        //    "LongTool.Commands.JohAbroad.HideLevelsHeadCommand",
        //    "Nhấn vào nút HideLevelsHead",
        //    $"{iconPath}Large/26-32.png",
        //    $"{iconPath}Small/26-16.png");

        // Management Panel Button ExpandCropTopToLevelCommand
        //buttonBuilder.AddPushButtonToPanel(panel,
        //    "btnExpandCropTopToLevelCommand", "ExpandCrop\nTopToLevel",
        //    "LongTool.Commands.JohAbroad.ExpandCropTopToLevelCommand",
        //    "Nhấn vào nút ExpandCropTopToLevel",
        //    $"{iconPath}Large/28-32.png",
        //    $"{iconPath}Small/28-16.png");

        // Management Panel Button CropBottomToLevelCommand
        //buttonBuilder.AddPushButtonToPanel(panel,
        //    "btnCropBottomToLevelCommand", "Crop\nBottomToLevel",
        //    "LongTool.Commands.JohAbroad.CropBottomToLevelCommand",
        //    "Nhấn vào nút CropBottomToLevelCommand",
        //    $"{iconPath}Large/29-32.png",
        //    $"{iconPath}Small/29-16.png");

        // Management Panel Button ExtendGridTopToViewCommand
        //buttonBuilder.AddPushButtonToPanel(panel,
        //    "btnAdjustGridTopToCropTopCommand", "AdjustGrid\nTopToCrop\nTopCommand",
        //    "LongTool.Commands.JohAbroad.AdjustGridTopToCropTopCommand",
        //    "Nhấn vào nút AdjustGridTopToCropTop",
        //    $"{iconPath}Large/30-32.png",
        //    $"{iconPath}Small/30-16.png");

        // Management Panel Button DrawWallCenterLineCommand
        //buttonBuilder.AddPushButtonToPanel(panel,
        //    "btnDrawWallCenterLineCommand", "DrawWall\nCenterLine",
        //    "LongTool.Commands.JohAbroad.DrawWallCenterLineCommand",
        //    "Nhấn vào nút DrawWallCenterLine",
        //    $"{iconPath}Large/31-32.png",
        //    $"{iconPath}Small/31-16.png");

        //panel.AddSeparator();

        // 1. Split Button với Icon
        var splitOptions01 = new List<(string id, string text, string commandPath, string tooltip, string largeIcon, string smallIcon)>
        {
            ("splitOpt1", "Hide\nLevelsHead", "LongTool.Commands.JohAbroad.HideLevelsHeadCommand",
                "Ẩn đi Ký hiệu Levels!",
                $"{iconPath}Large/7-32.png", $"{iconPath}Small/7-16.png"),
            ("splitOpt2", "Expand\nCrop Top\nTo Level", "LongTool.Commands.JohAbroad.ExpandCropTopToLevelCommand",
                "Mở rộng cạnh trên vùng Crop View đến Level gần nhất!",
                $"{iconPath}Large/8-32.png", $"{iconPath}Small/8-16.png"),
            ("splitOpt3", "Crop\nBottom ToLevel", "LongTool.Commands.JohAbroad.CropBottomToLevelCommand",
                "Mở rộng cạnh dưới vùng Crop View đến Level(phía trên) gần nhất!",
                $"{iconPath}Large/3-32.png", $"{iconPath}Small/3-16.png"),
            ("splitOpt4", "Adjust\nGrid Top\nTo CropTop", "LongTool.Commands.JohAbroad.AdjustGridTopToCropTopCommand",
                "Điều chỉnh đầu trục trên Trục bản vẽ đến cạnh trên vùng Crop View!",
                $"{iconPath}Large/4-32.png", $"{iconPath}Small/4-16.png"),
            ("splitOpt5", "Draw Wall\nCenter Line", "LongTool.Commands.JohAbroad.DrawWallCenterLineCommand",
                "Vẽ Tâm tường!",
                $"{iconPath}Large/5-32.png", $"{iconPath}Small/5-16.png")
        };
        buttonBuilder.CreateSplitButton(panel, "splitBtn", "Thao tác", splitOptions01, "Các thao tác cơ bản");

        // 2. Split Button với Icon
        var splitOptions02 = new List<(string id, string text, string commandPath, string tooltip, string largeIcon, string smallIcon)>
        {
            ("splitOpt02-01", "Set Grid\nBubbles", "LongTool.Commands.JohAbroad.Grid.SetGridBubblesCommand",
                "Vẽ Tâm tường!",
                $"{iconPath}Large/6-32.png", $"{iconPath}Small/6-16.png")
        };
        buttonBuilder.CreateSplitButton(panel, "GridsplitBtn", "Thao tác Grid", splitOptions02, "Các thao tác cơ bản điều chỉnh Grid");

        // 2. Split Button với Icon
        var splitOptions03 = new List<(string id, string text, string commandPath, string tooltip, string largeIcon, string smallIcon)>
        {
            ("splitOpt03-01", "DoorSchedule", "LongTool.Commands.JohAbroad.DoorScheduleCommand",
                "Thống kê cửa!",
                $"{iconPath}Large/7-32.png", $"{iconPath}Small/7-16.png")
        };
        buttonBuilder.CreateSplitButton(panel, "DoorScheduleBtn", "Thống kê cửa", splitOptions03, "Các thao tác về Cửa");
    }
}