namespace ConsoleNexusEngine.Internal;

using System.Runtime.CompilerServices;

[InlineArray(NexusColorPalette.MaxColorCount)]
internal struct ColorSet
{
    private NexusColor _element;
}
