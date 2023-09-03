namespace ConsoleNexusEngine.Internal.Models;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal unsafe struct CONSOLE_FONT_INFO_EX
{
    public int cbSize;
    public int nFont;
    public short FontWidth;
    public short FontHeight;
    public int FontFamily;
    public int FontWeight;
    public fixed char FaceName[32];
}