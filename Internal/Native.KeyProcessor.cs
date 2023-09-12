namespace ConsoleNexusEngine.Internal;

using ConsoleNexusEngine.Common;
using System.Runtime.InteropServices;

internal static partial class Native
{
    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial short GetAsyncKeyState(int vKey);

    public static bool IsKeyPressed(NexusKey key)
        => (GetAsyncKeyState((int)key) & 0x8000) != 0;
}