namespace ConsoleNexusEngine.Common;

public sealed partial record ColorPalette
{
    /// <summary>
    /// The color palette of IBM's original Color Graphics Adapter<br/>
    /// <see href="https://en.wikipedia.org/wiki/Color_Graphics_Adapter"/>
    /// </summary>
    public static ColorPalette CGA { get; }

    /// <summary>
    /// The color palette of the ZX Spectrum computer series (only 15 colors)<br/>
    /// <see href="https://en.wikipedia.org/wiki/ZX_Spectrum"/>
    /// </summary>
    public static ColorPalette ZXSpectrum { get; }

    /// <summary>
    /// The color palette of the Atari ST<br/>
    /// <see href="https://en.wikipedia.org/wiki/Atari_ST"/>
    /// </summary>
    public static ColorPalette AtariST { get; }

    /// <summary>
    /// The color palette of the MSX (only 15 colors)<br/>
    /// <see href="https://en.wikipedia.org/wiki/MSX2"/>
    /// </summary>
    public static ColorPalette MSX { get; }

    static ColorPalette()
    {
        CGA = new(stackalloc NexusColor[]
        {
            new(0x000000),
            new(0x0000AA),
            new(0x00AA00),
            new(0x00AAAA),
            new(0xAA0000),
            new(0xAA00AA),
            new(0xAA5500),
            new(0xAAAAAA),
            new(0x555555),
            new(0x5555FF),
            new(0x55FF55),
            new(0x55FFFF),
            new(0xFF5555),
            new(0xFF55FF),
            new(0xFFFF55),
            new(0xFFFFFF)
        });

        ZXSpectrum = new(stackalloc NexusColor[]
        {
            new(0x000000),
            new(0x0000D8),
            new(0x0000FF),
            new(0xD80000),
            new(0xFF0000),
            new(0xD800D8),
            new(0xFF00FF),
            new(0x00D800),
            new(0x00FF00),
            new(0x00D8D8),
            new(0x00FFFF),
            new(0xD8D800),
            new(0xFFFF00),
            new(0xD8D8D8),
            new(0xFFFFFF),
            new()
        });

        AtariST = new(stackalloc NexusColor[]
        {
            new(0x000000),
            new(0x606060),
            new(0xA0A0A0),
            new(0xE0E0E0),
            new(0xE0E020),
            new(0xA06000),
            new(0x20E0E0),
            new(0x00A0A0),
            new(0xE020E0),
            new(0xA000A0),
            new(0x20E020),
            new(0x00A000),
            new(0xE08080),
            new(0xA00000),
            new(0x6060E0),
            new(0x0000A0)
        });

        MSX = new(stackalloc NexusColor[]
        {
            new(0x000000),
            new(0xCACACA),
            new(0xFFFFFF),
            new(0xB75E51),
            new(0xD96459),
            new(0xFE877C),
            new(0xCAC15E),
            new(0xDDCE85),
            new(0x3CA042),
            new(0x40B64A),
            new(0x73CE7C),
            new(0x5955DF),
            new(0x7E75F0),
            new(0x64DAEE),
            new(0xB565B3),
            new()
        });
    }
}