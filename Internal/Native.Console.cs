namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;
using System.Text;

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

    [DllImport(User32)]
    public static extern int ShowWindow(nint hWnd, int nCmdShow);

    [DllImport(User32)]
    public static extern int GetWindowTextLength(nint hWnd);

    [DllImport(User32)]
    public static extern int GetWindowText(nint hWnd, StringBuilder title, int maxCount);

    [DllImport(User32)]
    public static extern bool SetWindowText(nint hWnd, string title);

    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowRect(nint hWnd, ref RECT lpRect);

    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport(Kernel32)]
    public static extern bool GetConsoleCursorInfo(nint hConsoleOutput, out CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

    [DllImport(Kernel32)]
    public static extern bool SetConsoleCursorInfo(nint hConsoleOutput, [In] ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

    [DllImport(Kernel32)]
    public static extern bool GetConsoleScreenBufferInfoEx(nint hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [DllImport(Kernel32)]
    public static extern bool SetConsoleScreenBufferInfoEx(nint hConsoleOutput, [In] ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [LibraryImport(User32)]
    public static partial int GetSystemMetrics(int nIndex);

    [DllImport(User32)]
    public static extern int GetWindowLong(nint hWnd, int nIndex);

    [DllImport(User32)]
    public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetForegroundWindow(nint hWnd);

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

    [DllImport(Kernel32, ExactSpelling = true)]
    public static extern bool WriteConsoleOutputW(
        [In] nint hConsoleOutput,
        [In] CHAR_INFO* lpBuffer,
        [In] COORD dwBufferSize,
        [In] COORD dwBufferCoord,
        [In] SMALL_RECT* lpWriteRegion);
}