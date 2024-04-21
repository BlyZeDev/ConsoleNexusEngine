namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents the console font 'Terminal'
/// </summary>
public sealed record TerminalFont : NexusFont
{
    /// <summary>
    /// Initializes a new Terminal
    /// </summary>
    /// <param name="width">The width of the font</param>
    /// <param name="height">The height of the font</param>
    public TerminalFont(int width, int height)
        : base("Terminal", width, height) { }

    /// <summary>
    /// Initializes a new Terminal
    /// </summary>
    /// <param name="dimensions">The width and height of the font</param>
    public TerminalFont(int dimensions) : this(dimensions, dimensions) { }
}