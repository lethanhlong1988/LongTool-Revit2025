namespace LongTool.Domain.Interfaces;

/// <summary>
/// Trừu tượng hóa việc đọc Parameter từ một Element.
/// Implement thực tế nằm ở RevitAdapters (dùng Revit API).
/// Domain chỉ biết interface này, không biết Revit.
/// </summary>
public interface IParameterReader
{
    /// <summary>
    /// Đọc parameter dạng text. Trả về chuỗi rỗng nếu không có.
    /// </summary>
    string GetText(string parameterName);

    /// <summary>
    /// Đọc parameter dạng số (length). Trả về 0 nếu không có.
    /// Đơn vị trả về là millimeters.
    /// </summary>
    double GetLengthInMillimeters(string parameterName);
}