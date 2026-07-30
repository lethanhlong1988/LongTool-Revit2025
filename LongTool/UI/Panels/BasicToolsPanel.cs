using System.Collections.Generic;
using Autodesk.Revit.UI;

namespace LongTool.UI.Panels;

public class BasicToolsPanel
{
    public void Build(RibbonPanel panel, string assemblyPath)
    {
        var buttonBuilder = new Buttons.ButtonBuilder(assemblyPath);

        // Đường dẫn icon (tương đối từ thư mục DLL)
        string iconPath = "Resources/Icons/";

        // 1. Nút lớn - Lời chào với Icon
        buttonBuilder.AddPushButtonToPanel(panel,
            "btnHello", "Lời chào",
            "LongTool.Commands.Basic.CommandHello",
            "Hiển thị lời chào thân thiện",
            $"{iconPath}Large/02-32.png",
            $"{iconPath}Small/02-16.png");

        // 2. Nút xếp chồng 2 nút với Icon
        buttonBuilder.AddTwoStackedButtonsToPanel(panel,
            ("btnInfo", "Thông tin", "LongTool.Commands.Basic.CommandInfo",
                "Hiển thị thông tin chi tiết",
                $"{iconPath}Large/info.png", $"{iconPath}Small/08-16.png"),
            ("btnSetting", "Cài đặt", "LongTool.Commands.Basic.CommandSetting",
                "Mở cửa sổ cài đặt",
                $"{iconPath}Large/setting.png", $"{iconPath}Small/19-16.png")
        );

        // Separator
        panel.AddSeparator();

        // 3. Nút xếp chồng 3 nút với Icon
        buttonBuilder.AddThreeStackedButtonsToPanel(panel,
            ("stack1", "CN A", "LongTool.Commands.Basic.CommandHello",
                "Chức năng A",
                $"{iconPath}Large/icon_a.png", $"{iconPath}Small/26-16.png"),
            ("stack2", "CN B", "LongTool.Commands.Basic.CommandInfo",
                "Chức năng B",
                $"{iconPath}Large/icon_b.png", $"{iconPath}Small/27-16.png"),
            ("stack3", "CN C", "LongTool.Commands.Basic.CommandSetting",
                "Chức năng C",
                $"{iconPath}Large/icon_c.png", $"{iconPath}Small/28-16.png")
        );

        panel.AddSeparator();

        // 4. Split Button với Icon
        var splitOptions = new List<(string id, string text, string commandPath, string tooltip, string largeIcon, string smallIcon)>
        {
            ("splitOpt1", "Tạo mới", "LongTool.Commands.Basic.CommandHello",
                "Tạo đối tượng mới",
                $"{iconPath}Large/32-32.png", $"{iconPath}Small/32-16.png"),
            ("splitOpt2", "Chỉnh sửa", "LongTool.Commands.Basic.CommandInfo",
                "Chỉnh sửa đối tượng",
                $"{iconPath}Large/33-32.png", $"{iconPath}Small/33-16.png"),
            ("splitOpt3", "Xóa bỏ", "LongTool.Commands.Basic.CommandSetting",
                "Xóa đối tượng",
                $"{iconPath}Large/34-32.png", $"{iconPath}Small/34-16.png")
        };
        buttonBuilder.CreateSplitButton(panel, "splitBtn", "Thao tác", splitOptions, "Các thao tác cơ bản");

        // 5. Pulldown Button (không có Icon hoặc có thể thêm nếu muốn)
        var pullOptions = new List<(string id, string text, string commandPath, string tooltip, string largeIcon, string smallIcon)>
        {
            ("pullOpt1", "Export dữ liệu", "LongTool.Commands.Basic.CommandHello", "Xuất dữ liệu ra file",  $"{iconPath}Large/34-32.png", $"{iconPath}Small/34-16.png"),
            ("pullOpt2", "Import dữ liệu", "LongTool.Commands.Basic.CommandInfo", "Nhập dữ liệu từ file",  $"{iconPath}Large/34-32.png", $"{iconPath}Small/34-16.png"),
            ("pullOpt3", "Xử lý hàng loạt", "LongTool.Commands.Basic.CommandSetting", "Xử lý hàng loạt đối tượng",  $"{iconPath}Large/34-32.png", $"{iconPath}Small/34-16.png")
        };
        buttonBuilder.CreatePulldownButton(panel, "pullBtn", "Công cụ", pullOptions, "Danh sách công cụ mở rộng");

    }
}