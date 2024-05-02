namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents a character in the console
/// </summary>
public readonly record struct NexusChar
{
    /// <summary>
    /// The character itself
    /// </summary>
    public readonly char Value { get; }

    /// <summary>
    /// The foreground color of the text
    /// </summary>
    public readonly NexusColor Foreground { get; }

    /// <summary>
    /// The background color of the text, <see langword="null"/> if the console background color should be used
    /// </summary>
    public readonly NexusColor? Background { get; }

    /// <summary>
    /// Initializes a new console character
    /// </summary>
    public NexusChar() : this(char.MinValue, NexusColor.Black) { }

    /// <summary>
    /// Initializes a new console character
    /// </summary>
    /// <param name="value">The character itself</param>
    /// <param name="foreground">The foreground color of the character</param>
    /// <param name="background">The background color of the character, <see langword="null"/> if the console background color should be used</param>
    public NexusChar(in char value, in NexusColor foreground, in NexusColor? background = null)
    {
        Value = value;
        Foreground = foreground;
        Background = background;
    }

    /// <summary>
    /// Initializes a new console character
    /// </summary>
    /// <param name="value">The character itself</param>
    /// <param name="foreground">The foreground color of the character</param>
    /// <param name="background">The background color of the character, <see langword="null"/> if the console background color should be used</param>
    public NexusChar(in NexusSpecialChar value, in NexusColor foreground, in NexusColor? background = null)
    {
        Value = (char)value;
        Foreground = foreground;
        Background = background;
    }

    internal static NexusChar FromGlyph(in Glyph glyph, NexusColorPalette colorPalette)
        => new(glyph.Value, colorPalette[glyph.ForegroundIndex], colorPalette[glyph.BackgroundIndex]);
}