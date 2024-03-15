namespace ConsoleNexusEngine.Internal.WIN;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct CONSOLE_SCREEN_BUFFER_INFO_EX
{
    internal int cbSize;
    internal COORD dwSize;
    internal COORD dwCursorPosition;
    internal ushort wAttributes;
    internal SMALL_RECT srWindow;
    internal COORD dwMaximumWindowSize;
    internal ushort wPopupAttributes;
    internal bool bFullscreenSupported;
    internal COLORREF black;
    internal COLORREF darkBlue;
    internal COLORREF darkGreen;
    internal COLORREF darkCyan;
    internal COLORREF darkRed;
    internal COLORREF darkMagenta;
    internal COLORREF darkYellow;
    internal COLORREF gray;
    internal COLORREF darkGray;
    internal COLORREF blue;
    internal COLORREF green;
    internal COLORREF cyan;
    internal COLORREF red;
    internal COLORREF magenta;
    internal COLORREF yellow;
    internal COLORREF white;
}