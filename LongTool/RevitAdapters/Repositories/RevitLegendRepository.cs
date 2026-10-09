using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace LongTool.RevitAdapters.Repositories;

/// <summary>
/// Truy vấn các Legend View trong Document.
/// Tất cả method đều dùng chung filter: ViewType == Legend && !IsTemplate.
/// </summary>
internal class RevitLegendRepository
{
    private readonly Document _doc;

    public RevitLegendRepository(Document doc) => _doc = doc;

    // ============================================================
    // PRIVATE — Query cơ sở, dùng chung filter
    // ============================================================

    /// <summary>
    /// Query tất cả Legend View (không phải template).
    /// Đây là nguồn dữ liệu duy nhất cho mọi method public.
    /// </summary>
    private IReadOnlyList<View> QueryAll()
    {
        return new FilteredElementCollector(_doc)
            .OfClass(typeof(View))
            .Cast<View>()
            .Where(v => v.ViewType == ViewType.Legend && !v.IsTemplate)
            .ToList();
    }

    // ============================================================
    // ACTIVE VIEW
    // ============================================================

    /// <summary>
    /// Trả về Legend View đang active, hoặc null nếu không phải Legend.
    /// </summary>
    public View? GetActiveLegendOrNull()
    {
        var view = _doc.ActiveView;
        return view.ViewType == ViewType.Legend ? view : null;
    }

    // ============================================================
    // QUERY
    // ============================================================

    /// <summary>
    /// Có ít nhất 1 Legend View (không phải template) không.
    /// </summary>
    public bool HasAny()
    {
        return QueryAll().Count > 0;
    }

    /// <summary>
    /// Lấy tất cả Legend View (không phải template).
    /// </summary>
    public IReadOnlyList<View> GetAll()
    {
        return QueryAll();
    }

    /// <summary>
    /// Lấy tên tất cả Legend View (không phải template).
    /// </summary>
    public IReadOnlyList<string> GetAllLegendNames()
    {
        return QueryAll().Select(v => v.Name).ToList();
    }

    // ============================================================
    // LOOKUP
    // ============================================================

    /// <summary>
    /// Kiểm tra Legend có tồn tại theo tên (không phân biệt hoa thường).
    /// </summary>
    public bool Exists(string name)
    {
        return QueryAll()
            .Any(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Tìm Legend theo tên (không phân biệt hoa thường).
    /// </summary>
    public View? FindByName(string name)
    {
        return QueryAll()
            .FirstOrDefault(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Tìm Legend theo tên (khớp chính xác, có phân biệt hoa thường).
    /// </summary>
    public View? FindByNameExact(string name)
    {
        return QueryAll()
            .FirstOrDefault(v => v.Name == name);
    }
}