namespace ConsoleNexusEngine.Internal.Models;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct CONSOLE_CURSOR_INFO
{
    public uint dwSize;
    public int bVisible;
}