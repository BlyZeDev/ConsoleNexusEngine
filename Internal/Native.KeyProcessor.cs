namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal static partial class Native
{
    [LibraryImport("user32.dll")]
    public static partial short GetAsyncKeyState(int vKey);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetNumberOfConsoleInputEvents(nint hConsoleInput, out int lpNumberOfEvents);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PeekConsoleInput(nint hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, int nLength, out int lpNumberOfEventsRead);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FlushConsoleInputBuffer(nint hConsoleInput);

    [DllImport("kernel32.dll")]
    public static extern bool WriteConsoleInput(nint hConsoleInput, [In] INPUT_RECORD[] lpBuffer, uint nLength, out uint lpNumberOfEventsWritten);
}