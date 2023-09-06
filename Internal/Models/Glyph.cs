namespace ConsoleNexusEngine.Internal.Models;

using ConsoleNexusEngine.Common;

internal record struct Glyph
{
    public char Value { get; set; }
    public NexusColor Foreground { get; set; }
    public NexusColor Background { get; set; }

    public Glyph(char value, NexusColor foreground, NexusColor background)
    {
        Value = value;
        Foreground = foreground;
        Background = background;
    }
}