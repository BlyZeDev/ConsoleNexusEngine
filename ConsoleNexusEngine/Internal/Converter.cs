namespace ConsoleNexusEngine.Internal;

internal static class Converter
{
    public static CHAR_INFO ToCharInfo(in NexusChar character)
    {
        return new CHAR_INFO
        {
            Attributes = (short)(character.Foreground | character.Background << 4),
            UnicodeChar = character.Value
        };
    }

    public static NexusChar ToNexusChar(CHAR_INFO charInfo)
        => new NexusChar(charInfo.UnicodeChar, new NexusColorIndex(charInfo.Attributes & 0x0F), new NexusColorIndex((charInfo.Attributes >> 4) & 0x0F));
}