namespace LongTool.Tables.Services;

public readonly struct TextMeasureResult
{
    public static readonly TextMeasureResult Empty =
        new(0, 0);

    public double Width { get; }

    public double Height { get; }

    public TextMeasureResult(
        double width,
        double height)
    {
        Width = width;
        Height = height;
    }
}