namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// A pixel of a <see cref="NexusSpriteMap"/>
/// </summary>
public readonly record struct NexusSpriteMapPixel
{
    /// <summary>
    /// The character
    /// </summary>
    public readonly char Character { get; }

    /// <summary>
    /// The foreground color of the character
    /// </summary>
    public readonly NexusColorIndex Foreground { get; }

    /// <summary>
    /// The background color of the character
    /// </summary>
    public readonly NexusColorIndex Background { get; }

    /// <summary>
    /// Initializes a <see cref="NexusSpriteMapPixel"/>
    /// </summary>
    /// <param name="character">The character of the pixel</param>
    /// <param name="foreground">The foreground color index of the pixel</param>
    /// <param name="background">The background color index of the pixel</param>
    public NexusSpriteMapPixel(char character, in NexusColorIndex foreground, in NexusColorIndex background)
    {
        Character = character;
        Foreground = foreground;
        Background = background;
    }
}