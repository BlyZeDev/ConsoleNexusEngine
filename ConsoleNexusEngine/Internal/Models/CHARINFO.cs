namespace ConsoleNexusEngine.Internal.Models;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
internal struct CHARINFO
{
    [FieldOffset(0)]
    public char UnicodeChar;
    [FieldOffset(2)]
    public short Attributes;
}