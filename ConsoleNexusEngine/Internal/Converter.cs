namespace ConsoleNexusEngine.Internal;

internal static class Converter
{
    public static CHAR_INFO ToCharInfo(in NexusChar character) => ToCharInfo(character.Value, character.Foreground, character.Background);

    public static CHAR_INFO ToCharInfo(in char character, in int foreground, in int background)
    {
        return new CHAR_INFO
        {
            Attributes = (short)(foreground | background << 4),
            UnicodeChar = character
        };
    }

    public static NexusChar ToNexusChar(CHAR_INFO charInfo)
        => new NexusChar(charInfo.UnicodeChar, new NexusColorIndex(charInfo.Attributes & 0x0F), new NexusColorIndex((charInfo.Attributes >> 4) & 0x0F));
}