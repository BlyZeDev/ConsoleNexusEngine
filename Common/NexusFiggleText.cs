namespace ConsoleNexusEngine.Common;

using ConsoleNexusEngine.Internal.Models;
using Figgle;
using System;
using System.Linq;

/// <summary>
/// Represents a figgle text in the console
/// </summary>
public sealed record NexusFiggleText : INexusColored
{
    internal readonly int _longestStringLength;

    /// <summary>
    /// The text lines itself
    /// </summary>
    public string[] Value { get; }

    /// <inheritdoc/>
    public NexusColor Foreground { get; }

    /// <inheritdoc/>
    public NexusColor? Background { get; }

    /// <summary>
    /// The figgle font of the text
    /// </summary>
    public FiggleFont FiggleFont { get; }

    /// <summary>
    /// Initializes a new console text in figgle font
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="figgleFont">The Figgle Font that is used on the <paramref name="value"/></param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="background">The background color of the text, <see langword="null"/> if the console background color should be used</param>
    public NexusFiggleText(string value, FiggleFont figgleFont, NexusColor foreground, NexusColor? background = null)
    {
        (Value, _longestStringLength) = InitText(value, figgleFont);
        Foreground = foreground;
        Background = background;
        FiggleFont = figgleFont;
    }

    /// <summary>
    /// Initializes a new console text in figgle font
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="figgleFont">The Figgle Font that is used on the <paramref name="value"/></param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="background">The background color of the text, <see langword="null"/> if the console background color should be used</param>
    public NexusFiggleText(char value, FiggleFont figgleFont, NexusColor foreground, NexusColor? background = null)
        : this(value.ToString(), figgleFont, foreground, background) { }

    /// <summary>
    /// Initializes a new console text in figgle font
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="figgleFont">The Figgle Font that is used on the <paramref name="value"/></param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="background">The background color of the text, <see langword="null"/> if the console background color should be used</param>
    public NexusFiggleText(object value, FiggleFont figgleFont, NexusColor foreground, NexusColor? background = null)
        : this(value?.ToString() ?? "", figgleFont, foreground, background) { }

    private static (string[], int) InitText(string value, FiggleFont font)
    {
        value = font.Render(value);

        var values = value.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        return (values, values.Aggregate("", (max, val) => val.Length > max.Length ? val : max).Length);
    }
}