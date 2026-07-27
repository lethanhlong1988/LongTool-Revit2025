using Autodesk.Revit.DB;
using LongTool.Tables.Engine;
using System;

using EngineTableCellStyle = LongTool.Tables.Engine.TableCellStyle;

namespace LongTool.Tables.Renderer;

/// <summary>
/// Chịu trách nhiệm render nội dung text của TableCell.
/// </summary>
internal sealed class TextRenderer
{
    #region Constants

    /// <summary>
    /// Offset nhỏ để tránh text chạm vào border (mm).
    /// </summary>
    private const double TextOffset = 1.0;

    #endregion

    #region Fields

    private readonly RevitRenderContext _context;
    private readonly RevitTextStyleManager _styleManager;

    #endregion

    #region Constructor

    public TextRenderer(RevitRenderContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _styleManager = new RevitTextStyleManager(context.Document);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Render toàn bộ text trong bảng.
    /// </summary>
    public void Render(TableLayout layout)
    {
        if (layout == null)
            throw new ArgumentNullException(nameof(layout));

        if (!layout.IsBuilt)
            throw new InvalidOperationException(
                "TableLayout chưa được Build(). Hãy gọi layout.Build() trước khi render.");

        foreach (TableCellLayout cell in layout.Cells)
        {
            RenderCell(cell);
        }
    }

    #endregion

    #region Cell Rendering

    /// <summary>
    /// Render text cho một cell.
    /// </summary>
    private void RenderCell(TableCellLayout cell)
    {
        if (cell == null) return;

        string text = cell.Cell.Text;
        if (string.IsNullOrWhiteSpace(text)) return;

        // Tính vị trí đặt text
        XYZ position = CalculateTextPosition(cell);

        // Tạo TextNote
        CreateTextNote(position, text, cell.Cell.Style);
    }

    #endregion

    #region Position Calculation

    /// <summary>
    /// Tính vị trí đặt text trong cell.
    /// Xử lý đúng padding và alignment.
    /// </summary>
    private XYZ CalculateTextPosition(TableCellLayout cell)
    {
        var style = cell.Cell.Style;

        double x;
        double y;

        // --- Horizontal Alignment ---
        switch (style.HorizontalAlignment)
        {
            case TableHorizontalAlignment.Left:
                x = cell.Left + style.PaddingLeft + TextOffset;
                break;

            case TableHorizontalAlignment.Right:
                x = cell.Right - style.PaddingRight - TextOffset;
                break;

            case TableHorizontalAlignment.Center:
            default:
                x = cell.CenterX;
                break;
        }

        // --- Vertical Alignment ---
        // LƯU Ý: TableEngine sử dụng gốc tọa độ ở góc dưới trái
        // Top > Bottom (Y tăng dần lên trên)
        switch (style.VerticalAlignment)
        {
            case TableVerticalAlignment.Top:
                // Top = Y cao nhất, đi xuống (trừ padding và offset)
                y = cell.Top - style.PaddingTop - TextOffset;
                break;

            case TableVerticalAlignment.Bottom:
                // Bottom = Y thấp nhất, đi lên (cộng padding và offset)
                y = cell.Bottom + style.PaddingBottom + TextOffset;
                break;

            case TableVerticalAlignment.Middle:
            default:
                y = cell.CenterY;
                break;
        }

        // Chuyển sang tọa độ Revit
        return _context.ToXYZ(new TablePoint(x, y));
    }

    #endregion

    #region Text Creation

    /// <summary>
    /// Tạo TextNote trong Revit.
    /// </summary>
    private void CreateTextNote(XYZ position, string text, EngineTableCellStyle style)
    {
        if (position == null || string.IsNullOrWhiteSpace(text)) return;

        try
        {
            // Lấy TextNoteType phù hợp với style
            ElementId textTypeId = _styleManager.GetTextType(style.Text);

            // Tạo TextNoteOptions
            TextNoteOptions options = new TextNoteOptions(textTypeId);

            // Áp dụng Alignment
            ApplyAlignment(options, style);

            // Tạo TextNote
            TextNote note = TextNote.Create(
                _context.Document,
                _context.View.Id,
                position,
                text,
                options);

            if (note == null) return;

            // Áp dụng màu chữ (nếu cần)
            ApplyTextColor(note, style.Text.TextColor);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Không thể tạo TextNote: {ex.Message}");
        }
    }

    #endregion

    #region Text Style Application

    /// <summary>
    /// Áp dụng Alignment cho TextNoteOptions.
    /// </summary>
    private void ApplyAlignment(TextNoteOptions options, EngineTableCellStyle style)
    {
        if (options == null || style == null) return;

        // Horizontal Alignment
        switch (style.HorizontalAlignment)
        {
            case TableHorizontalAlignment.Left:
                options.HorizontalAlignment = HorizontalTextAlignment.Left;
                break;

            case TableHorizontalAlignment.Right:
                options.HorizontalAlignment = HorizontalTextAlignment.Right;
                break;

            case TableHorizontalAlignment.Center:
            default:
                options.HorizontalAlignment = HorizontalTextAlignment.Center;
                break;
        }

        // Vertical Alignment
        switch (style.VerticalAlignment)
        {
            case TableVerticalAlignment.Top:
                options.VerticalAlignment = VerticalTextAlignment.Top;
                break;

            case TableVerticalAlignment.Bottom:
                options.VerticalAlignment = VerticalTextAlignment.Bottom;
                break;

            case TableVerticalAlignment.Middle:
            default:
                options.VerticalAlignment = VerticalTextAlignment.Middle;
                break;
        }
    }

    /// <summary>
    /// Áp dụng màu chữ cho TextNote.
    /// </summary>
    private void ApplyTextColor(TextNote note, TableColor color)
    {
        if (note == null) return;

        try
        {
            // Nếu màu đen (mặc định), không cần override
            if (color.Equals(TableColor.Black))
                return;

            // ✅ SỬA LỖI: Dùng Fully Qualified Name
            Autodesk.Revit.DB.Color revitColor = new Autodesk.Revit.DB.Color(
                color.R,
                color.G,
                color.B);

            // Tạo OverrideGraphicSettings
            OverrideGraphicSettings settings = new OverrideGraphicSettings();

            // Set màu chữ cho Projection
            settings.SetProjectionLineColor(revitColor);

            // Apply override cho TextNote
            _context.View.SetElementOverrides(note.Id, settings);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Không thể áp dụng màu chữ: {ex.Message}");
        }
    }

    #endregion
}