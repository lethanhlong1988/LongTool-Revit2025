using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace LongTool.UI.Buttons;

public class ButtonBuilder
{
    private readonly string _assemblyPath;
    private readonly string _resourcePath;

    public ButtonBuilder(string assemblyPath)
    {
        _assemblyPath = assemblyPath;
        _resourcePath = Path.GetDirectoryName(assemblyPath);
    }

    // Helper load icon với kiểm tra kích thước
    private BitmapImage LoadImage(string relativePath, int targetSize = 0)
    {
        if (string.IsNullOrEmpty(relativePath))
            return null;

        try
        {
            var fullPath = Path.Combine(_resourcePath, relativePath);
            if (!File.Exists(fullPath))
                return null;

            var uri = new Uri(fullPath, UriKind.Absolute);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = uri;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;

            if (targetSize > 0)
            {
                bitmap.DecodePixelWidth = targetSize;
                bitmap.DecodePixelHeight = targetSize;
            }

            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    // Tạo PushButton với Icon
    public PushButton AddPushButtonToPanel(RibbonPanel panel, string id, string text,
        string commandPath, string tooltip = null,
        string largeIconPath = null, string smallIconPath = null)
    {
        var data = new PushButtonData(id, text, _assemblyPath, commandPath);

        if (!string.IsNullOrEmpty(tooltip))
            data.ToolTip = tooltip;

        if (!string.IsNullOrEmpty(largeIconPath))
        {
            var largeImage = LoadImage(largeIconPath, 32);
            if (largeImage != null)
                data.LargeImage = largeImage;
        }

        if (!string.IsNullOrEmpty(smallIconPath))
        {
            var smallImage = LoadImage(smallIconPath, 16);
            if (smallImage != null)
                data.Image = smallImage;
        }

        return panel.AddItem(data) as PushButton;
    }

    // Stacked buttons
    public IList<RibbonItem> AddTwoStackedButtonsToPanel(RibbonPanel panel,
        (string id, string text, string commandPath, string tooltip, string largeIcon, string smallIcon) btn1,
        (string id, string text, string commandPath, string tooltip, string largeIcon, string smallIcon) btn2)
    {
        var data1 = CreatePushButtonDataWithIcon(btn1.id, btn1.text, btn1.commandPath,
            btn1.tooltip, btn1.largeIcon, btn1.smallIcon);

        var data2 = CreatePushButtonDataWithIcon(btn2.id, btn2.text, btn2.commandPath,
            btn2.tooltip, btn2.largeIcon, btn2.smallIcon);

        return panel.AddStackedItems(data1, data2);
    }

    public IList<RibbonItem> AddThreeStackedButtonsToPanel(RibbonPanel panel,
        (string id, string text, string commandPath, string tooltip, string largeIcon, string smallIcon) btn1,
        (string id, string text, string commandPath, string tooltip, string largeIcon, string smallIcon) btn2,
        (string id, string text, string commandPath, string tooltip, string largeIcon, string smallIcon) btn3)
    {
        var data1 = CreatePushButtonDataWithIcon(btn1.id, btn1.text, btn1.commandPath,
            btn1.tooltip, btn1.largeIcon, btn1.smallIcon);

        var data2 = CreatePushButtonDataWithIcon(btn2.id, btn2.text, btn2.commandPath,
            btn2.tooltip, btn2.largeIcon, btn2.smallIcon);

        var data3 = CreatePushButtonDataWithIcon(btn3.id, btn3.text, btn3.commandPath,
            btn3.tooltip, btn3.largeIcon, btn3.smallIcon);

        return panel.AddStackedItems(data1, data2, data3);
    }

    private PushButtonData CreatePushButtonDataWithIcon(string id, string text, string commandPath,
        string tooltip, string largeIconPath, string smallIconPath)
    {
        var data = new PushButtonData(id, text, _assemblyPath, commandPath);

        if (!string.IsNullOrEmpty(tooltip))
            data.ToolTip = tooltip;

        if (!string.IsNullOrEmpty(largeIconPath))
        {
            var largeImage = LoadImage(largeIconPath, 32);
            if (largeImage != null)
                data.LargeImage = largeImage;
        }

        if (!string.IsNullOrEmpty(smallIconPath))
        {
            var smallImage = LoadImage(smallIconPath, 16);
            if (smallImage != null)
                data.Image = smallImage;
        }

        return data;
    }

    // Split Button với Icon
    public SplitButton CreateSplitButton(RibbonPanel panel, string id, string text,
        List<(string id, string text, string commandPath, string tooltip, string largeIcon, string smallIcon)> options,
        string tooltip = null)
    {
        var data = new SplitButtonData(id, text);
        if (!string.IsNullOrEmpty(tooltip))
            data.ToolTip = tooltip;

        var splitButton = panel.AddItem(data) as SplitButton;
        if (splitButton != null)
        {
            foreach (var opt in options)
            {
                var optData = new PushButtonData(opt.id, opt.text, _assemblyPath, opt.commandPath);
                if (!string.IsNullOrEmpty(opt.tooltip))
                    optData.ToolTip = opt.tooltip;

                if (!string.IsNullOrEmpty(opt.largeIcon))
                {
                    var largeImage = LoadImage(opt.largeIcon, 32);
                    if (largeImage != null)
                        optData.LargeImage = largeImage;
                }
                if (!string.IsNullOrEmpty(opt.smallIcon))
                {
                    var smallImage = LoadImage(opt.smallIcon, 16);
                    if (smallImage != null)
                        optData.Image = smallImage;
                }

                splitButton.AddPushButton(optData);
            }
            splitButton.IsSynchronizedWithCurrentItem = true;
        }
        return splitButton;
    }

    // Pulldown Button
    public PulldownButton CreatePulldownButton(
    RibbonPanel panel,
    string id,
    string text,
    List<(
        string id,
        string text,
        string commandPath,
        string tooltip,
        string largeIcon,
        string smallIcon)> items,
    string tooltip = null)
    {
        var data = new PulldownButtonData(id, text);

        if (!string.IsNullOrEmpty(tooltip))
            data.ToolTip = tooltip;

        var pullButton = panel.AddItem(data) as PulldownButton;

        if (pullButton != null)
        {
            foreach (var item in items)
            {
                var itemData = new PushButtonData(
                    item.id,
                    item.text,
                    _assemblyPath,
                    item.commandPath);

                if (!string.IsNullOrEmpty(item.tooltip))
                    itemData.ToolTip = item.tooltip;

                if (!string.IsNullOrEmpty(item.largeIcon))
                {
                    var largeImage = LoadImage(item.largeIcon, 32);
                    if (largeImage != null)
                        itemData.LargeImage = largeImage;
                }

                if (!string.IsNullOrEmpty(item.smallIcon))
                {
                    var smallImage = LoadImage(item.smallIcon, 16);
                    if (smallImage != null)
                        itemData.Image = smallImage;
                }

                pullButton.AddPushButton(itemData);
            }
        }

        return pullButton;
    }
}