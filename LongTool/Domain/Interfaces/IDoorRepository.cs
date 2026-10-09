using System.Collections.Generic;
using LongTool.Models;   // namespace cũ của DoorScheduleItem

namespace LongTool.Domain.Interfaces;

/// <summary>
/// Nguồn cung cấp dữ liệu Door đã được chuẩn hóa.
/// Implement ở tầng RevitAdapters.
/// </summary>
public interface IDoorRepository
{
    IReadOnlyList<DoorScheduleItem> GetAll();
}