namespace ConsoleNexusEngine.Internal.WIN;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct CONSOLE_CURSOR_INFO
{
    public uint dwSize;
    public bool bVisible;
}