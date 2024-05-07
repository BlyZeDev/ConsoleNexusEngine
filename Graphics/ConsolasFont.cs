namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents the console font 'Consolas'
/// </summary>
public sealed record ConsolasFont : NexusFont
{
    /// <summary>
    /// Initializes a new Consolas
    /// </summary>
    /// <param name="width">The width of the font</param>
    /// <param name="height">The height of the font</param>
    public ConsolasFont(in int width, in int height)
        : base("Consolas", width, height) { }

    /// <summary>
    /// Initializes a new Consolas
    /// </summary>
    /// <param name="dimensions">The width and height of the font</param>
    public ConsolasFont(in int dimensions) : this(dimensions, dimensions) { }
}