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
            $"{iconPath}Large/hello.png",
            $"{iconPath}Small/hello.png");

        // 2. Nút xếp chồng 2 nút với Icon
        buttonBuilder.AddTwoStackedButtonsToPanel(panel,
            ("btnInfo", "Thông tin", "LongTool.Commands.Basic.CommandInfo",
                "Hiển thị thông tin chi tiết",
                $"{iconPath}Large/info.png", $"{iconPath}Small/info.png"),
            ("btnSetting", "Cài đặt", "LongTool.Commands.Basic.CommandSetting",
                "Mở cửa sổ cài đặt",
                $"{iconPath}Large/setting.png", $"{iconPath}Small/setting.png")
        );

        // Separator
        panel.AddSeparator();

        // 3. Nút xếp chồng 3 nút với Icon
        buttonBuilder.AddThreeStackedButtonsToPanel(panel,
            ("stack1", "CN A", "LongTool.Commands.Basic.CommandHello",
                "Chức năng A",
                $"{iconPath}Large/icon_a.png", $"{iconPath}Small/icon_a.png"),
            ("stack2", "CN B", "LongTool.Commands.Basic.CommandInfo",
                "Chức năng B",
                $"{iconPath}Large/icon_b.png", $"{iconPath}Small/icon_b.png"),
            ("stack3", "CN C", "LongTool.Commands.Basic.CommandSetting",
                "Chức năng C",
                $"{iconPath}Large/icon_c.png", $"{iconPath}Small/icon_c.png")
        );

        panel.AddSeparator();

        // 4. Split Button với Icon
        var splitOptions = new List<(string id, string text, string commandPath, string tooltip, string largeIcon, string smallIcon)>
        {
            ("splitOpt1", "Tạo mới", "LongTool.Commands.Basic.CommandHello",
                "Tạo đối tượng mới",
                $"{iconPath}Large/new.png", $"{iconPath}Small/new.png"),
            ("splitOpt2", "Chỉnh sửa", "LongTool.Commands.Basic.CommandInfo",
                "Chỉnh sửa đối tượng",
                $"{iconPath}Large/edit.png", $"{iconPath}Small/edit.png"),
            ("splitOpt3", "Xóa bỏ", "LongTool.Commands.Basic.CommandSetting",
                "Xóa đối tượng",
                $"{iconPath}Large/delete.png", $"{iconPath}Small/delete.png")
        };
        buttonBuilder.CreateSplitButton(panel, "splitBtn", "Thao tác", splitOptions, "Các thao tác cơ bản");

        // 5. Pulldown Button (không có Icon hoặc có thể thêm nếu muốn)
        var pullOptions = new List<(string id, string text, string commandPath, string tooltip)>
        {
            ("pullOpt1", "Export dữ liệu", "LongTool.Commands.Basic.CommandHello", "Xuất dữ liệu ra file"),
            ("pullOpt2", "Import dữ liệu", "LongTool.Commands.Basic.CommandInfo", "Nhập dữ liệu từ file"),
            ("pullOpt3", "Xử lý hàng loạt", "LongTool.Commands.Basic.CommandSetting", "Xử lý hàng loạt đối tượng")
        };
        buttonBuilder.CreatePulldownButton(panel, "pullBtn", "Công cụ", pullOptions, "Danh sách công cụ mở rộng");

    }
}