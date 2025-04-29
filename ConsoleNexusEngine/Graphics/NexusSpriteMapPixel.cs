namespace ConsoleNexusEngine.Graphics;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// A pixel of a <see cref="NexusSpriteMap"/>
/// </summary>
public readonly record struct NexusSpriteMapPixel
{
    /// <summary>
    /// The character
    /// </summary>
    public required readonly char Character { get; init; }

    /// <summary>
    /// The foreground color of the character
    /// </summary>
    public required readonly NexusColorIndex Foreground { get; init; }

    /// <summary>
    /// The background color of the character
    /// </summary>
    public required readonly NexusColorIndex Background { get; init; }

    /// <summary>
    /// Initializes a <see cref="NexusSpriteMapPixel"/>
    /// </summary>
    /// <param name="character">The character of the pixel</param>
    /// <param name="foreground">The foreground color index of the pixel</param>
    /// <param name="background">The background color index of the pixel</param>
    [SetsRequiredMembers]
    public NexusSpriteMapPixel(char character, in NexusColorIndex foreground, in NexusColorIndex background)
    {
        Character = character;
        Foreground = foreground;
        Background = background;
    }
}