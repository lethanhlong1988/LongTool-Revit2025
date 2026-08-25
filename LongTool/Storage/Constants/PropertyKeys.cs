using System.Collections.Generic;

namespace LongTool.Storage.Constants;

public static class PropertyKeys
{
    public const string Family = "Family";

    public const string Type = "Type";

    public const string Level = "Level";

    public const string Width = "Width";

    public const string Height = "Height";

    public const string Thickness = "Thickness";


    public static readonly HashSet<string>
        HeaderProperties =
        new()
        {
            Family,
            Type,
            Level,
            Width,
            Height,
            Thickness
        };
}