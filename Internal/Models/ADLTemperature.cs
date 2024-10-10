namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct ADLTemperature
{
    public int Size;
    public int Temperature;
}