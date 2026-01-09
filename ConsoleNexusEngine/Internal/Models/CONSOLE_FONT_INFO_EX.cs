namespace ConsoleNexusEngine.Internal.Models;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal unsafe struct CONSOLE_FONT_INFO_EX
{
    public const int FACE_NAME_SIZE = 32;

    public uint cbSize;
    public uint nFont;
    public COORD dwFontSize;
    public int FontFamily;
    public int FontWeight;
    public fixed char FaceName[FACE_NAME_SIZE];
}