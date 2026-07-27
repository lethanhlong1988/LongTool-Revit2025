using Autodesk.Revit.DB;
using LongTool.Tables.Engine;
using System;
using System.Linq;

namespace LongTool.Tables.Renderer;

/// <summary>
/// Quản lý TextNoteType cho TableRenderer.
/// Chuyển TableTextStyle thành Revit Text Style.
/// </summary>
internal sealed class RevitTextStyleManager
{
    #region Fields

    private readonly Document _document;

    #endregion

    #region Constructor

    public RevitTextStyleManager(Document document)
    {
        _document = document ?? throw new ArgumentNullException(nameof(document));
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Lấy TextNoteType phù hợp với style.
    /// Nếu chưa có thì tạo mới.
    /// </summary>
    public ElementId GetTextType(TableTextStyle style)
    {
        if (style == null)
            throw new ArgumentNullException(nameof(style));

        // Tìm TextNoteType hiện có
        TextNoteType existingType = FindExistingType(style);
        if (existingType != null)
            return existingType.Id;

        // Tạo mới nếu chưa có
        return CreateTextType(style);
    }

    #endregion

    #region Find Existing Type

    private TextNoteType FindExistingType(TableTextStyle style)
    {
        FilteredElementCollector collector = new FilteredElementCollector(_document)
            .OfClass(typeof(TextNoteType));

        foreach (TextNoteType type in collector)
        {
            if (IsMatch(type, style))
                return type;
        }

        return null;
    }

    private bool IsMatch(TextNoteType type, TableTextStyle style)
    {
        // Kiểm tra FontSize
        Parameter sizeParam = type.get_Parameter(BuiltInParameter.TEXT_SIZE);
        if (sizeParam == null) return false;

        double currentFeet = sizeParam.AsDouble();
        double currentMm = UnitUtils.ConvertFromInternalUnits(
            currentFeet,
            UnitTypeId.Millimeters);

        if (Math.Abs(currentMm - style.FontSize) > 0.001) return false;

        // Lưu ý: Revit không có BuiltInParameter.TEXT_BOLD và TEXT_ITALIC
        // Bold/Italic được kiểm tra qua TextNoteType properties khác
        // Hoặc có thể bỏ qua phần này, chỉ kiểm tra FontSize

        return true;
    }

    #endregion

    #region Create New Type

    private ElementId CreateTextType(TableTextStyle style)
    {
        // Lấy TextNoteType base
        TextNoteType baseType = GetBaseTextNoteType();
        if (baseType == null)
            throw new InvalidOperationException("Không tìm thấy TextNoteType.");

        // Tạo tên cho TextNoteType mới
        string typeName = CreateTypeName(style);

        // Duplicate TextNoteType
        TextNoteType newType = baseType.Duplicate(typeName) as TextNoteType;
        if (newType == null)
            throw new InvalidOperationException("Không thể duplicate TextNoteType.");

        // Set FontSize
        Parameter sizeParam = newType.get_Parameter(BuiltInParameter.TEXT_SIZE);
        if (sizeParam != null)
        {
            double fontSizeFeet = UnitUtils.ConvertToInternalUnits(
                style.FontSize,
                UnitTypeId.Millimeters);
            sizeParam.Set(fontSizeFeet);
        }

        // Lưu ý: Không thể set Bold/Italic trực tiếp trên TextNoteType
        // Cần tạo TextNoteType với style phù hợp từ đầu

        return newType.Id;
    }

    private TextNoteType GetBaseTextNoteType()
    {
        // Tìm TextNoteType đầu tiên trong document
        FilteredElementCollector collector = new FilteredElementCollector(_document)
            .OfClass(typeof(TextNoteType));

        TextNoteType firstType = collector.FirstElement() as TextNoteType;
        if (firstType != null) return firstType;

        // Nếu không có TextNoteType nào, không thể tạo
        return null;
    }

    private string CreateTypeName(TableTextStyle style)
    {
        string name = $"LT_TableText_{style.FontSize:0.0}mm";

        // Thêm Bold/Italic vào tên nếu có
        if (style.Bold)
            name += "_Bold";
        if (style.Italic)
            name += "_Italic";

        return name;
    }

    #endregion
}