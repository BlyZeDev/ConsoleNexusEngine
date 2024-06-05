namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents the console font 'Consolas'
/// </summary>
public sealed record ConsolasFont : NexusFont
{
    /// <summary>
    /// Initializes a new Consolas font
    /// </summary>
    /// <param name="size">The size of the font</param>
    public ConsolasFont(in NexusSize size) : base("Consolas", size) { }
}