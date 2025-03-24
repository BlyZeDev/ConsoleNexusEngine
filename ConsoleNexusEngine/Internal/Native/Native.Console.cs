namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal static unsafe partial class Native
{
    [LibraryImport(Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AllocConsole();

    [LibraryImport(Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FreeConsole();

    [LibraryImport(Kernel32)]
    public static partial nint GetStdHandle(int nStdHandle);

    [LibraryImport(Kernel32)]
    public static partial nint GetConsoleWindow();

    [DllImport(Kernel32)]
    public static extern bool GetConsoleCursorInfo(nint hConsoleOutput, out CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

    [DllImport(Kernel32)]
    public static extern bool SetConsoleCursorInfo(nint hConsoleOutput, [In] ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

    [DllImport(Kernel32)]
    public static extern bool GetConsoleScreenBufferInfoEx(nint hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [DllImport(Kernel32)]
    public static extern bool SetConsoleScreenBufferInfoEx(nint hConsoleOutput, [In] ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [LibraryImport(Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetConsoleMode(nint hConsoleHandle, out uint dwMode);

    [LibraryImport(Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

    [DllImport(Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCurrentConsoleFontEx(nint hConsoleOutput, [MarshalAs(UnmanagedType.Bool)] bool bMaximumWindow, out CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);

    [DllImport(Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCurrentConsoleFontEx(nint hConsoleOutput, [MarshalAs(UnmanagedType.Bool)] bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);

    [DllImport(Kernel32, CharSet = CharSet.Unicode)]
    public static extern bool WriteConsoleOutput(
        [In] nint hConsoleOutput,
        [In] CHARINFO* lpBuffer,
        [In] COORD dwBufferSize,
        [In] COORD dwBufferCoord,
        [In] SMALL_RECT* lpWriteRegion);
}