namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// The default windows console color palette
/// </summary>
[IncludeColorPalette]
public sealed record DefaultColorPalette : NexusColorPalette
{
    /// <summary>
    /// The default windows console color palette
    /// </summary>
    public DefaultColorPalette()
    {
        Colors =
        [
            new NexusColor(0x000000),
            new NexusColor(0x000080),
            new NexusColor(0x008000),
            new NexusColor(0x800000),
            new NexusColor(0x008080),
            new NexusColor(0x800080),
            new NexusColor(0x808000),
            new NexusColor(0xC0C0C0),
            new NexusColor(0x808080),
            new NexusColor(0x0000FF),
            new NexusColor(0x00FF00),
            new NexusColor(0xFF0000),
            new NexusColor(0x00FFFF),
            new NexusColor(0xFF00FF),
            new NexusColor(0xFFFF00),
            new NexusColor(0xFFFFFF)
        ];
    }
}