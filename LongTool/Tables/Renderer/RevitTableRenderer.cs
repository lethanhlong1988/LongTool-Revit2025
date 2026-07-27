using LongTool.Tables.Engine;

namespace LongTool.Tables.Renderer;

/// <summary>
/// Renderer chính của bảng trong Revit.
/// </summary>
public sealed class RevitTableRenderer : ITableRenderer
{
    private readonly RevitRenderContext _context;

    private readonly BorderRenderer _borderRenderer;
    private readonly TextRenderer _textRenderer;
    private readonly CrossRenderer _crossRenderer;  // ✅ Thêm CrossRenderer

    public RevitTableRenderer(RevitRenderContext context)
    {
        _context = context ?? throw new System.ArgumentNullException(nameof(context));

        _borderRenderer = new BorderRenderer(_context);
        _textRenderer = new TextRenderer(_context);
        _crossRenderer = new CrossRenderer(_context);  // ✅ Khởi tạo CrossRenderer
    }

    /// <summary>
    /// Render bảng dựa trên layout đã được tính toán.
    /// </summary>
    public void Render(TableLayout layout)
    {
        if (layout == null)
            throw new System.ArgumentNullException(nameof(layout));

        // Render đường viền (bao gồm cả gạch chéo)
        _borderRenderer.Render(layout);  // ✅ BorderRenderer đã gọi CrossRenderer bên trong

        // Render text
        _textRenderer.Render(layout);
    }
}