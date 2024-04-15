namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents a font for the console game
/// </summary>
public sealed partial record NexusFont
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

    private NexusFont(string name, in int width, in int height)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1, nameof(width));
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1, nameof(height));

        Name = name;
        Width = width;
        Height = height;
    }
}