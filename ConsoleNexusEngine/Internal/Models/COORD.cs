namespace ConsoleNexusEngine.Internal.Models;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct COORD
{
    public short X;
    public short Y;
};