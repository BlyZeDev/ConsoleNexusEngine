﻿namespace ConsoleNexusEngine.Graphics;

using System.Collections.Immutable;

/// <summary>
/// The color palette of the MSX (only 15 colors)<br/>
/// <see href="https://en.wikipedia.org/wiki/MSX2"/>
/// </summary>
public sealed record MSXColorPalette : NexusColorPalette
{
    /// <inheritdoc/>
    protected override ImmutableArray<NexusColor> Colors => [
        new NexusColor(0x000000),
        new NexusColor(0xCACACA),
        new NexusColor(0xFFFFFF),
        new NexusColor(0xB75E51),
        new NexusColor(0xD96459),
        new NexusColor(0xFE877C),
        new NexusColor(0xCAC15E),
        new NexusColor(0xDDCE85),
        new NexusColor(0x3CA042),
        new NexusColor(0x40B64A),
        new NexusColor(0x73CE7C),
        new NexusColor(0x5955DF),
        new NexusColor(0x7E75F0),
        new NexusColor(0x64DAEE),
        new NexusColor(0xB565B3),
        new NexusColor(0xF0F0F0)];
}