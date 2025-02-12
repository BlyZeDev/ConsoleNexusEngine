namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
internal struct CHAR_INFO
{
    [FieldOffset(0)]
    public char UnicodeChar;
    [FieldOffset(2)]
    public short Attributes;
}