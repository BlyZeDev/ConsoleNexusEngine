namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents a text in the console
/// </summary>
public sealed record NexusText
{
    /// <summary>
    /// The text itself
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// The foreground color of the text
    /// </summary>
    public NexusColor Foreground { get; }

    /// <summary>
    /// The background color of the text, <see langword="null"/> if the console background color should be used
    /// </summary>
    public NexusColor? Background { get; }

    /// <summary>
    /// The flow direction of the text
    /// </summary>
    public NexusTextDirection TextDirection { get; }

    /// <summary>
    /// Initializes a new console text
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="background">The background color of the text, <see langword="null"/> if the console background color should be used</param>
    /// <param name="textDirection">The flow direction of the text</param>
    public NexusText(string value, in NexusColor foreground, in NexusColor? background = null, in NexusTextDirection textDirection = NexusTextDirection.Horizontal)
    {
        Value = value;
        Foreground = foreground;
        Background = background;
        TextDirection = textDirection;
    }

    /// <summary>
    /// Initializes a new console text
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="background">The background color of the text, <see langword="null"/> if the console background color should be used</param>
    /// <param name="textDirection">The flow direction of the text</param>
    public NexusText(object? value, in NexusColor foreground, in NexusColor? background = null, in NexusTextDirection textDirection = NexusTextDirection.Horizontal)
        : this(value?.ToString() ?? "", foreground, background, textDirection) { }
}