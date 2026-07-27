using LongTool.Tables.Engine;

namespace LongTool.Tables.Renderer;

/// <summary>
/// Định nghĩa renderer cho bảng.
/// </summary>
public interface ITableRenderer
{
    /// <summary>
    /// Render bảng từ layout đã được tính toán.
    /// </summary>
    /// <param name="layout">
    /// TableLayout đã Build().
    /// </param>
    void Render(
        TableLayout layout);
}