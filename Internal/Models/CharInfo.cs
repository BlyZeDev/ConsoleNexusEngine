namespace ConsoleNexusEngine.Internal.Models;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
internal struct CharInfo
{
    [FieldOffset(0)]
    public char UnicodeChar;
    [FieldOffset(0)]
    public byte AsciiChar;
    [FieldOffset(2)]
    public short Attributes;
}