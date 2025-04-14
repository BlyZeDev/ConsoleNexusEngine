namespace ConsoleNexusEngine.Graphics;

using System.Collections.Immutable;

/// <summary>
/// The color palette of the Pico-8<br/>
/// <see href="https://de.wikipedia.org/wiki/Pico-8"/>
/// </summary>
[IncludeColorPalette]
public sealed record Pico8ColorPalette : NexusColorPalette
{
    /// <inheritdoc/>
    protected override ImmutableArray<NexusColor> Colors => [
        new NexusColor(0x000000),
        new NexusColor(0x1D2B53),
        new NexusColor(0x7E2553),
        new NexusColor(0x008751),
        new NexusColor(0xAB5236),
        new NexusColor(0x5F574F),
        new NexusColor(0xC2C3C7),
        new NexusColor(0xFFF1E8),
        new NexusColor(0xFF004D),
        new NexusColor(0xFFA300),
        new NexusColor(0xFFEC27),
        new NexusColor(0x00E436),
        new NexusColor(0x29ADFF),
        new NexusColor(0x83769C),
        new NexusColor(0xFF77A8),
        new NexusColor(0xFFCCAA)];
}