namespace ConsoleNexusEngine.Common;

/// <summary>
/// Represents a character in the console
/// </summary>
public record struct NexusChar
{
    /// <summary>
    /// The character itself
    /// </summary>
    public char Value { get; set; }

    /// <summary>
    /// The foreground color of the character
    /// </summary>
    public NexusColor Foreground { get; set; }

    /// <summary>
    /// The background color of the character
    /// </summary>
    public NexusColor? Background { get; set; }

    /// <summary>
    /// Initializes a new console character
    /// </summary>
    /// <param name="value">The character itself</param>
    /// <param name="foreground">The foreground color of the character</param>
    /// <param name="background">The background color of the character, <see langword="null"/> if the console background color should be used</param>
    public NexusChar(char value, NexusColor foreground, NexusColor? background = null)
    {
        Value = value;
        Foreground = foreground;
        Background = background;
    }
}