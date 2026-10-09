using System.Collections.Generic;
using System.Text;
using LongTool.Models;

namespace LongTool.Domain.Services;

/// <summary>
/// Build chuỗi summary để hiển thị confirm. Thuần C#, dễ test.
/// </summary>
public static class DoorBoardSummaryBuilder
{
    public static string BuildGroupingSummary(
        int totalDoorCount,
        IReadOnlyList<DoorScheduleItem> grouped)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Tìm thấy {totalDoorCount} cửa.");
        sb.AppendLine();
        sb.AppendLine($"Có {grouped.Count} loại cửa:");
        sb.AppendLine();

        foreach (var door in grouped)
            sb.AppendLine($"{door.Symbol}    × {door.Quantity}");

        sb.AppendLine();
        sb.Append("Bạn có muốn tiếp tục không?");
        return sb.ToString();
    }

    public static string BuildCreatedSummary(
        IReadOnlyList<string> createdLegendNames,
        string templateName)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Đã tạo {createdLegendNames.Count} Legend:");
        sb.AppendLine();

        foreach (var name in createdLegendNames)
            sb.AppendLine($"• {name}");

        sb.AppendLine();
        sb.AppendLine($"Template: {templateName}");
        sb.AppendLine();
        sb.Append("Đã tạo bảng Door Board tương ứng cho từng Legend.");
        return sb.ToString();
    }
}