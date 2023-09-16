namespace ConsoleNexusEngine.Common;

using ConsoleNexusEngine.Internal;

/// <summary>
/// Represents a character in the console
/// </summary>
public readonly record struct NexusChar : INexusColored
{
    /// <summary>
    /// The character itself
    /// </summary>
    public char Value { get; }

    /// <inheritdoc/>
    public NexusColor Foreground { get; }

    /// <inheritdoc/>
    public NexusColor? Background { get; }

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

    /// <summary>
    /// Initializes a new console character
    /// </summary>
    /// <param name="value">The character itself</param>
    /// <param name="foreground">The foreground color of the character</param>
    /// <param name="background">The background color of the character, <see langword="null"/> if the console background color should be used</param>
    public NexusChar(NexusSpecialChar value, NexusColor foreground, NexusColor? background = null)
    {
        Value = (char)value;
        Foreground = foreground;
        Background = background;
    }
}