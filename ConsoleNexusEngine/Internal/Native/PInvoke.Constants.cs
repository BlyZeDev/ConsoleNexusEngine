namespace ConsoleNexusEngine.Internal.Native;

internal static partial class PInvoke
{
    private const string Kernel32 = "kernel32.dll";
    private const string User32 = "user32.dll";
    private const string XInput1_4 = "xinput1_4.dll";

    public const int STD_INPUT_HANDLE = -10;
    public const int STD_OUTPUT_HANDLE = -11;

    public const int SW_HIDE = 0;

    public const int GWL_STYLE = -16;

    public const int WS_BORDER = 0x00080000;
    public const int WS_MAXIMIZE = 0x00100000;
    public const int WS_MINIMIZE = 0x00200000;

    public const uint MB_TOPMOST = 0x00001000;

    public const nint HWND_NOTOPMOST = -2;

    public const uint SWP_SHOWWINDOW = 0x0040;
    public const uint SWP_FRAMECHANGED = 0x0020;

    public const uint ENABLE_EXTENDED_FLAGS = 0x0080;
    public const uint ENABLE_MOUSE_INPUT = 0x0010;
    public const uint ENABLE_ECHO_INPUT = 0x0004;
    public const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
    public const uint ENABLE_WINDOW_INPUT = 0x0008;

    public const uint MONITOR_DEFAULTTONEAREST = 1;

    public const int SM_CXSCREEN = 0;
    public const int SM_CYSCREEN = 1;

    public const uint CONSOLE_READ_NOWAIT = 0x0002;

    public const byte BATTERY_DEVTYPE_GAMEPAD = 0;

    public const ushort KEY_EVENT = 0x0001;
    public const ushort MOUSE_EVENT = 0x0002;

    public const uint MOUSE_MOVED = 0x0001;
    public const uint MOUSE_WHEELED = 0x0004;
    public const uint MOUSE_HWHEELED = 0x0008;

    public const uint FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001;
    public const uint RIGHTMOST_BUTTON_PRESSED = 0x0002;
    public const uint FROM_LEFT_2ND_BUTTON_PRESSED = 0x0004;
    public const uint FROM_LEFT_3RD_BUTTON_PRESSED = 0x0008;
    public const uint FROM_LEFT_4TH_BUTTON_PRESSED = 0x0010;

    public const uint MOUSE_WHEEL_DELTA = 0x80000000;
}