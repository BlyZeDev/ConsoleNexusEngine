namespace ConsoleNexusEngine.Graphics;

using System.Collections.Generic;

/// <summary>
/// The color palette of the Commodore 64<br/>
/// <see href="https://de.wikipedia.org/wiki/Commodore_64"/>
/// </summary>
public sealed class Commodore64ColorPalette : NexusColorPalette
{
    /// <inheritdoc/>
    protected override IReadOnlyList<NexusColor> Colors => [
        new NexusColor(0x000000),
        new NexusColor(0x626262),
        new NexusColor(0x898989),
        new NexusColor(0xADADAD),
        new NexusColor(0xFFFFFF),
        new NexusColor(0x9F4E44),
        new NexusColor(0xCB7E75),
        new NexusColor(0x6D5412),
        new NexusColor(0xA1683C),
        new NexusColor(0xC9D487),
        new NexusColor(0x9AE29B),
        new NexusColor(0x5CAB5E),
        new NexusColor(0x6ABFC6),
        new NexusColor(0x887ECB),
        new NexusColor(0x50459B),
        new NexusColor(0xA057A3)];
}