namespace ConsoleNexusEngine.Graphics;

using System.Collections.Immutable;

/// <summary>
/// The color palette of the old Windows<br/>
/// <see href="https://de.wikipedia.org/wiki/Microsoft_Windows_1.0"/>
/// </summary>
public sealed record WindowsColorPalette : NexusColorPalette
{
    /// <inheritdoc/>
    protected override ImmutableArray<NexusColor> Colors => [
        new NexusColor(0x000000),
        new NexusColor(0x7E7E7E),
        new NexusColor(0xBEBEBE),
        new NexusColor(0xFFFFFF),
        new NexusColor(0x7E0000),
        new NexusColor(0xFE0000),
        new NexusColor(0x047E00),
        new NexusColor(0x06FF04),
        new NexusColor(0x7E7E00),
        new NexusColor(0xFFFF04),
        new NexusColor(0x00007E),
        new NexusColor(0x0000FF),
        new NexusColor(0x7E007E),
        new NexusColor(0xFE00FF),
        new NexusColor(0x047E7E),
        new NexusColor(0x06FFFF)];
}