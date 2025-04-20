namespace ConsoleNexusEngine.Internal;

internal static class NativeConverter
{
    public static CHARINFO ToCharInfo(in NexusChar character) => ToCharInfo(character.Value, character.Foreground, character.Background);

    public static CHARINFO ToCharInfo(in NexusSpriteMapPixel pixel) => ToCharInfo(pixel.Character, pixel.Foreground, pixel.Background);

    public static CHARINFO ToCharInfo(char character, int foreground, int background)
    {
        return new CHARINFO
        {
            Attributes = (short)(foreground | background << 4),
            UnicodeChar = character
        };
    }

    public static NexusSpriteMapPixel ToSpriteMapPixel(CHARINFO charInfo)
        => new NexusSpriteMapPixel(charInfo.UnicodeChar, new NexusColorIndex(charInfo.Attributes & 0x0F), new NexusColorIndex((charInfo.Attributes >> 4) & 0x0F));

    public static NexusChar ToNexusChar(CHARINFO charInfo)
        => new NexusChar(charInfo.UnicodeChar, new NexusColorIndex(charInfo.Attributes & 0x0F), new NexusColorIndex((charInfo.Attributes >> 4) & 0x0F));

    public static NexusCoord ToNexusCoord(COORD coord) => new NexusCoord(coord.X, coord.Y);
}