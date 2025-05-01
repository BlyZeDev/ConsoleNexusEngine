namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// The color palette of IBM's original Color Graphics Adapter<br/>
/// <see href="https://en.wikipedia.org/wiki/Color_Graphics_Adapter"/>
/// </summary>
[IncludeColorPalette]
public sealed record CGAColorPalette : NexusColorPalette
{
    /// <summary>
    /// The color palette of IBM's original Color Graphics Adapter<br/>
    /// <see href="https://en.wikipedia.org/wiki/Color_Graphics_Adapter"/>
    /// </summary>
    public CGAColorPalette()
    {
        Colors =
        [
            new NexusColor(0x000000),
            new NexusColor(0x0000AA),
            new NexusColor(0x00AA00),
            new NexusColor(0x00AAAA),
            new NexusColor(0xAA0000),
            new NexusColor(0xAA00AA),
            new NexusColor(0xAA5500),
            new NexusColor(0xAAAAAA),
            new NexusColor(0x555555),
            new NexusColor(0x5555FF),
            new NexusColor(0x55FF55),
            new NexusColor(0x55FFFF),
            new NexusColor(0xFF5555),
            new NexusColor(0xFF55FF),
            new NexusColor(0xFFFF55),
            new NexusColor(0xFFFFFF)
        ];
    }
}