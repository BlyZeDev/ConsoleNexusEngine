namespace ConsoleNexusEngine.Common;

/// <summary>
/// Represents a character in the console
/// </summary>
public record struct NexusText
{
    /// <summary>
    /// The text itself
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// The foreground color of the text
    /// </summary>
    public NexusColor Foreground { get; set; }

    /// <summary>
    /// The background color of the text
    /// </summary>
    public NexusColor? Background { get; set; }

    /// <summary>
    /// The flow direction of the text
    /// </summary>
    public TextDirection FlowDirection { get; set; }

    /// <summary>
    /// Initializes a new console text
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="background">The background color of the text, <see langword="null"/> if the console background color should be used</param>
    /// <param name="flowDirection">The flow direction of the text</param>
    public NexusText(string value, NexusColor foreground, NexusColor? background, TextDirection flowDirection = TextDirection.Horizontal)
    {
        Value = value;
        Foreground = foreground;
        Background = background;
        FlowDirection = flowDirection;
    }
}