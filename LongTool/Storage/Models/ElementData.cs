using System.Collections.Generic;

namespace LongTool.Storage.Models;

public class ElementData
{
    public long Id { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string UniqueId { get; set; } = string.Empty;

    public List<ParameterData> Parameters { get; set; }
        = new();
}