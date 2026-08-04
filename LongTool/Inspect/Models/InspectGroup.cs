using LongTool.Storage.Models;
using System.Collections.Generic;

namespace LongTool.Inspect.Models;

public class InspectGroup
{
    public string Name { get; set; } = string.Empty;

    public List<ParameterData> Parameters { get; set; }
        = new();
}