namespace ConsoleNexusEngine.Common;

using ConsoleNexusEngine.Internal.Models;

/// <summary>
/// Represents a text in the console
/// </summary>
public sealed record NexusText : INexusColored
{
    /// <summary>
    /// The text itself
    /// </summary>
    public string Value { get; }

    /// <inheritdoc/>
    public NexusColor Foreground { get; }

    /// <inheritdoc/>
    public NexusColor? Background { get; }

    /// <summary>
    /// The flow direction of the text
    /// </summary>
    public TextDirection TextDirection { get; }

    /// <summary>
    /// Initializes a new console text
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="background">The background color of the text, <see langword="null"/> if the console background color should be used</param>
    /// <param name="textDirection">The flow direction of the text</param>
    public NexusText(string value, NexusColor foreground, NexusColor? background = null, TextDirection textDirection = TextDirection.Horizontal)
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
    public NexusText(object value, NexusColor foreground, NexusColor? background = null, TextDirection textDirection = TextDirection.Horizontal)
        : this(value?.ToString() ?? "", foreground, background, textDirection) { }
}