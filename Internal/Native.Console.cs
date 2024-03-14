namespace ConsoleNexusEngine.Internal;

using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Runtime.InteropServices;

internal static partial class Native
{
    [LibraryImport("kernel32.dll")]
    public static partial nint GetStdHandle(int nStdHandle);

    [LibraryImport("kernel32.dll")]
    public static partial nint GetConsoleWindow();

    [DllImport("user32.dll")]
    public static extern bool SetWindowText(nint hwnd, string title);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("kernel32.dll")]
    public static extern bool SetConsoleCursorInfo(nint hConsoleOutput, [In] ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

    [DllImport("kernel32.dll")]
    public static extern bool GetConsoleScreenBufferInfoEx(nint hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [DllImport("kernel32.dll")]
    public static extern bool SetConsoleScreenBufferInfoEx(nint hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowRect(nint hWnd, ref RECT lpRect);

    [LibraryImport("user32.dll")]
    public static partial int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetForegroundWindow(nint hWnd);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCurrentConsoleFontEx(nint hConsoleOutput, [MarshalAs(UnmanagedType.Bool)] bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            nint securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            nint template);

    [DllImport("kernel32.dll")]
    public static extern bool WriteConsoleOutputW(
        SafeFileHandle hConsoleOutput,
        CHAR_INFO[] lpBuffer,
        COORD dwBufferSize,
        COORD dwBufferCoord,
        ref SMALL_RECT lpWriteRegion);
}