namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents a font for the console game
/// </summary>
public sealed record NexusFont
{
    /// <summary>
    /// The name of the font
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The size of the font
    /// </summary>
    public NexusSize Size { get; }

    /// <summary>
    /// Initializes <see cref="NexusFont"/> from the specified font size
    /// </summary>
    /// <param name="name">The name of the font</param>
    /// <param name="size">The size of the font</param>
    /// <remarks>
    /// Keep in mind that some characters might not work in a font.<br/>
    /// I would recommend to test if the characters you need are included.
    /// </remarks>
    public NexusFont(string name, in NexusSize size)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size.Width, 1, nameof(size));
        ArgumentOutOfRangeException.ThrowIfLessThan(size.Height, 1, nameof(size));

        Name = name;
        Size = size;
    }
}