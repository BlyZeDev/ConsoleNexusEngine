namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents a font for the console game
/// </summary>
public abstract record NexusFont
{
    /// <summary>
    /// The name of the font
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The width of the font
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// The height of the font
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Initializes a new NexusFont
    /// </summary>
    /// <param name="name">The name of the font</param>
    /// <param name="width">The width of the font</param>
    /// <param name="height">The height of the font</param>
    /// <remarks>
    /// Keep in mind that some characters might not work in a font.<br/>
    /// I would recommend to test if the characters you need are included.
    /// </remarks>
    protected NexusFont(string name, int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1, nameof(width));
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1, nameof(height));

        Name = name;
        Width = width;
        Height = height;
    }
}