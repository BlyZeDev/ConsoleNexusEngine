namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct CONSOLE_SCREEN_BUFFER_INFO_EX
{
    public int cbSize;
    public COORD dwSize;
    public COORD dwCursorPosition;
    public ushort wAttributes;
    public SMALL_RECT srWindow;
    public COORD dwMaximumWindowSize;
    public ushort wPopupAttributes;
    public bool bFullscreenSupported;
    public COLORREF black;
    public COLORREF darkBlue;
    public COLORREF darkGreen;
    public COLORREF darkCyan;
    public COLORREF darkRed;
    public COLORREF darkMagenta;
    public COLORREF darkYellow;
    public COLORREF gray;
    public COLORREF darkGray;
    public COLORREF blue;
    public COLORREF green;
    public COLORREF cyan;
    public COLORREF red;
    public COLORREF magenta;
    public COLORREF yellow;
    public COLORREF white;
}