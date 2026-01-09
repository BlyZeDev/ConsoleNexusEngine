namespace ConsoleNexusEngine.Internal.Models;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct XINPUT_CAPABILITIES
{
    public byte Type;
    public byte SubType;
    public ushort Flags;
    public XINPUT_GAMEPAD Gamepad;
    public XINPUT_VIBRATION Vibration;
}