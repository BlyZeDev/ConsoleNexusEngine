namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal static unsafe partial class Native
{
    [LibraryImport(Kernel32)]
    public static partial nint GetStdHandle(int nStdHandle);

    [LibraryImport(Kernel32)]
    public static partial nint GetConsoleWindow();

    [DllImport(User32)]
    public static extern bool SetWindowText(nint hwnd, string title);

    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport(Kernel32)]
    public static extern bool SetConsoleCursorInfo(nint hConsoleOutput, [In] ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

    [DllImport(Kernel32)]
    public static extern bool GetConsoleScreenBufferInfoEx(nint hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [DllImport(Kernel32)]
    public static extern bool SetConsoleScreenBufferInfoEx(nint hConsoleOutput, [In] ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowRect(nint hWnd, ref RECT lpRect);

    [LibraryImport(User32)]
    public static partial int GetSystemMetrics(int nIndex);

    [DllImport(User32)]
    public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetForegroundWindow(nint hWnd);

    [LibraryImport(Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

    [DllImport(Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCurrentConsoleFontEx(nint hConsoleOutput, [MarshalAs(UnmanagedType.Bool)] bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);

    [DllImport(Kernel32, CharSet = CharSet.Unicode)]
    public static extern nint CreateFile(
            [In] char* fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            nint securityAttributes,
            [MarshalAs(UnmanagedType.U4)] uint creationDisposition,
            [MarshalAs(UnmanagedType.U4)] uint flags,
            nint template);

    [DllImport(Kernel32, ExactSpelling = true)]
    public static extern bool WriteConsoleOutputW(
        [In] nint hConsoleOutput,
        [In] CHAR_INFO* lpBuffer,
        [In] COORD dwBufferSize,
        [In] COORD dwBufferCoord,
        [In] SMALL_RECT* lpWriteRegion);
}