namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct SMALL_RECT
{
    public short Left;
    public short Top;
    public short Right;
    public short Bottom;
}