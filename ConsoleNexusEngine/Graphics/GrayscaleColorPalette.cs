namespace ConsoleNexusEngine.Graphics;

using System.Collections.Immutable;

/// <summary>
/// A grayscale color palette
/// </summary>
[IncludeColorPalette]
public sealed record GrayscaleColorPalette : NexusColorPalette
{
    /// <inheritdoc/>
    protected override ImmutableArray<NexusColor> Colors => [
        new NexusColor(0x000000),
        new NexusColor(0x181818),
        new NexusColor(0x282828),
        new NexusColor(0x383838),
        new NexusColor(0x474747),
        new NexusColor(0x565656),
        new NexusColor(0x646464),
        new NexusColor(0x717171),
        new NexusColor(0x7E7E7E),
        new NexusColor(0x8C8C8C),
        new NexusColor(0x9B9B9B),
        new NexusColor(0xABABAB),
        new NexusColor(0xBDBDBD),
        new NexusColor(0xD1D1D1),
        new NexusColor(0xE7E7E7),
        new NexusColor(0xFFFFFF)];
}