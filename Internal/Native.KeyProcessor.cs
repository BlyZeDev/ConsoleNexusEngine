namespace ConsoleNexusEngine.Internal;

using ConsoleNexusEngine.Common;
using System.Runtime.InteropServices;

internal static partial class Native
{
    public const int WH_MOUSE_LL = 14;
    public const int WH_KEYBOARD_LL = 13;
    public const int WM_LBUTTONDOWN = 0x0201;
    public const int WM_LBUTTONUP = 0x0202;
    public const int WM_RBUTTONDOWN = 0x0204;
    public const int WM_RBUTTONUP = 0x0205;
    public const int WM_KEYDOWN = 0x0100;
    public const int WM_KEYUP = 0x0101;

    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial short GetAsyncKeyState(int vKey);

    public static bool IsKeyPressed(GameInputKey key)
        => (GetAsyncKeyState((int)key) & 0x8000) != 0;
}