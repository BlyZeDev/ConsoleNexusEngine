namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents the console font 'Terminal'
/// </summary>
public sealed record Terminal : NexusFont
{
    /// <summary>
    /// Initializes a new Terminal
    /// </summary>
    /// <param name="width">The width of the font</param>
    /// <param name="height">The height of the font</param>
    public Terminal(int width, int height)
        : base("Terminal", width, height) { }

    /// <summary>
    /// Initializes a new Terminal
    /// </summary>
    /// <param name="dimensions">The width and height of the font</param>
    public Terminal(int dimensions) : this(dimensions, dimensions) { }
}