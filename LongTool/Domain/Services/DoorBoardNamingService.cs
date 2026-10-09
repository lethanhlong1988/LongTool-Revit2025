using System;

namespace LongTool.Domain.Services;

/// <summary>
/// Quản lý việc đặt tên Legend cho Door Board.
/// Logic thuần — không phụ thuộc Revit.
/// </summary>
public class DoorBoardNamingService
{
    /// <summary>
    /// Tạo tên Legend chính thức từ Symbol.
    /// Nếu Symbol rỗng → dùng fallback với timestamp.
    /// </summary>
    public string BuildTargetLegendName(string symbol, string fallbackPrefix = "DoorBoard_Test")
    {
        if (!string.IsNullOrWhiteSpace(symbol))
            return symbol;

        return $"{fallbackPrefix}_{DateTime.Now:HHmmss}";
    }

    /// <summary>
    /// Tạo tên Legend tạm (dùng khi Update — Legend cũ chưa xóa).
    /// Đảm bảo tên không trùng với bất kỳ Legend nào đã tồn tại.
    /// </summary>
    /// <param name="baseName">Tên gốc (thường là tên Legend chính thức)</param>
    /// <param name="exists">Hàm kiểm tra tên đã tồn tại chưa</param>
    public string BuildTemporaryLegendName(string baseName, Func<string, bool> exists)
    {
        var candidate = $"{baseName}_copy";
        int index = 1;

        while (exists(candidate))
        {
            candidate = $"{baseName}_copy_{index:00}";
            index++;
        }

        return candidate;
    }
}