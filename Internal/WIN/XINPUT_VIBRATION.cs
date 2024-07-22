namespace ConsoleNexusEngine.Internal.WIN;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct XINPUT_VIBRATION
{
    public ushort wLeftMotorSpeed;
    public ushort wRightMotorSpeed;
}