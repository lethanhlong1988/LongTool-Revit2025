using System;

namespace LongTool.Infrastructure;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CommandAttribute : Attribute
{
    public string Name { get; }
    public string Tooltip { get; set; }
    public string LargeImagePath { get; set; }
    public string SmallImagePath { get; set; }

    public CommandAttribute(string name)
    {
        Name = name;
    }
}