namespace ConsoleNexusEngine.Internal.Models;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
internal struct KEY_EVENT_RECORD
{
    [FieldOffset(0)]
    public bool KeyDown;
    [FieldOffset(4)]
    public ushort RepeatCount;
    [FieldOffset(6)]
    public ushort VirtualKeyCode;
    [FieldOffset(8)]
    public ushort VirtualScanCode;
    [FieldOffset(10)]
    public char UnicodeChar;
    [FieldOffset(12)]
    public int ControlKeyState;
}