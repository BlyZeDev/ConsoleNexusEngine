namespace ConsoleNexusEngine.Graphics;

using System.Collections.Generic;

/// <summary>
/// The color palette of the Atari ST<br/>
/// <see href="https://en.wikipedia.org/wiki/Atari_ST"/>
/// </summary>
public sealed class AtariSTColorPalette : NexusColorPalette
{
    /// <inheritdoc/>
    protected override IReadOnlyList<NexusColor> Colors => [
        new NexusColor(0x000000),
        new NexusColor(0x606060),
        new NexusColor(0xA0A0A0),
        new NexusColor(0xE0E0E0),
        new NexusColor(0xE0E020),
        new NexusColor(0xA06000),
        new NexusColor(0x20E0E0),
        new NexusColor(0x00A0A0),
        new NexusColor(0xE020E0),
        new NexusColor(0xA000A0),
        new NexusColor(0x20E020),
        new NexusColor(0x00A000),
        new NexusColor(0xE08080),
        new NexusColor(0xA00000),
        new NexusColor(0x6060E0),
        new NexusColor(0x0000A0)];
}