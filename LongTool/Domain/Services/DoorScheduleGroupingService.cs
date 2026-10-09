using System.Collections.Generic;
using System.Linq;
using LongTool.Models;

namespace LongTool.Domain.Services;

/// <summary>
/// Logic thuần: gom nhóm Door theo Symbol và merge thông tin.
/// Không phụ thuộc Revit → có thể unit test dễ dàng.
/// </summary>
public class DoorScheduleGroupingService
{
    public IReadOnlyList<DoorScheduleItem> GroupBySymbol(
        IEnumerable<DoorScheduleItem> items)
    {
        return items
            .Where(x => !string.IsNullOrWhiteSpace(x.Symbol))
            .GroupBy(x => x.Symbol)
            .Select(MergeGroup)
            .OrderBy(x => x.Symbol)
            .ToList();
    }

    private static DoorScheduleItem MergeGroup(
        IGrouping<string, DoorScheduleItem> group)
    {
        var first = group.First();
        return new DoorScheduleItem
        {
            Symbol = group.Key,
            Quantity = group.Count(),
            Type = first.Type,
            Location = first.Location,
            Glass = first.Glass,
            Finish = first.Finish,
            Hardware = first.Hardware,
            Remarks = first.Remarks,
            DoorThickness = first.DoorThickness,
            FrameThickness = first.FrameThickness,
            ShapeWidth = first.ShapeWidth,
            ShapeHeight = first.ShapeHeight
        };
    }
}