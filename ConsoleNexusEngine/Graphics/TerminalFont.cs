namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents the console font 'Terminal'
/// </summary>
public sealed record TerminalFont : NexusFont
{
    /// <summary>
    /// Initializes a new Terminal font
    /// </summary>
    /// <param name="size">The size of the font</param>
    public TerminalFont(in NexusSize size) : base("Terminal", size) { }
}