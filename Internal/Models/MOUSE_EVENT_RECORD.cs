namespace ConsoleNexusEngine.Internal.Models;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
internal struct MOUSE_EVENT_RECORD
{
    [FieldOffset(0)]
    public COORD MousePosition;
    [FieldOffset(4)]
    public uint ButtonState;
    [FieldOffset(8)]
    public uint ControlKeyState;
    [FieldOffset(12)]
    public uint EventFlags;
}