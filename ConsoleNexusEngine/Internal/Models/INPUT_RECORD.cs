namespace ConsoleNexusEngine.Internal.Models;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
internal struct INPUT_RECORD
{
    [FieldOffset(0)]
    public ushort EventType;
    [FieldOffset(4)]
    public KEY_EVENT_RECORD KeyEvent;
    [FieldOffset(4)]
    public MOUSE_EVENT_RECORD MouseEvent;
}