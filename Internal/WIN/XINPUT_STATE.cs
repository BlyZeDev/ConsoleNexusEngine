namespace ConsoleNexusEngine.Internal.WIN;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct XINPUT_STATE
{
    public uint dwPacketNumber;
    public XINPUT_GAMEPAD Gamepad;
}