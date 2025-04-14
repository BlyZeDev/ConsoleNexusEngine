namespace ConsoleNexusEngine.Graphics;

using System.Collections.Immutable;

/// <summary>
/// The color palette of the ZX Spectrum computer series (only 15 colors)<br/>
/// <see href="https://en.wikipedia.org/wiki/ZX_Spectrum"/>
/// </summary>
[IncludeColorPalette]
public sealed record ZXSpectrumColorPalette : NexusColorPalette
{
    /// <inheritdoc/>
    protected override ImmutableArray<NexusColor> Colors => [
        new NexusColor(0x000000),
        new NexusColor(0x0000D8),
        new NexusColor(0x0000FF),
        new NexusColor(0xD80000),
        new NexusColor(0xFF0000),
        new NexusColor(0xD800D8),
        new NexusColor(0xFF00FF),
        new NexusColor(0x00D800),
        new NexusColor(0x00FF00),
        new NexusColor(0x00D8D8),
        new NexusColor(0x00FFFF),
        new NexusColor(0xD8D800),
        new NexusColor(0xFFFF00),
        new NexusColor(0xD8D8D8),
        new NexusColor(0xFFFFFF),
        new NexusColor(0x000000)];
}