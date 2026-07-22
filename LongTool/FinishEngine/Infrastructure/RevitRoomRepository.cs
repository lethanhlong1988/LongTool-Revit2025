using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using LongTool.FinishEngine.Domain;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LongTool.FinishEngine.Infrastructure;

/// <summary>
/// Cung cấp các phương thức truy xuất Room từ mô hình Revit.
/// </summary>
public class RevitRoomRepository
{
    private readonly RevitRoomMapper _mapper = new();

    /// <summary>
    /// Lấy tất cả Room trong Document và chuyển thành FinishRoom.
    /// </summary>
    /// <param name="document">Tài liệu Revit.</param>
    /// <returns>Danh sách FinishRoom.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public IReadOnlyList<FinishRoom> GetRooms(Document document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));

        return new FilteredElementCollector(document)
            .OfCategory(BuiltInCategory.OST_Rooms)
            .WhereElementIsNotElementType()
            .Cast<Room>()
            .Select(room => _mapper.Map(room))
            .ToList();
    }
}