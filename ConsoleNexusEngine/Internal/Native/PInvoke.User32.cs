namespace ConsoleNexusEngine.Internal.Native;

using System.Runtime.InteropServices;

internal static partial class PInvoke
{
    [LibraryImport(User32, SetLastError = true)]
    public static partial nint MonitorFromWindow(nint hwnd, uint dwFlags);

    [LibraryImport(User32, EntryPoint = "GetMonitorInfoW", SetLastError = true)]
    public static partial int GetMonitorInfo(nint hMonitor, ref MONITORINFO lpmi);

    [LibraryImport(User32, SetLastError = true)]
    public static partial int ShowWindow(nint hWnd, int nCmdShow);

    [LibraryImport(User32, EntryPoint = "GetWindowTextLengthW", SetLastError = true)]
    public static partial int GetWindowTextLength(nint hWnd);

    [LibraryImport(User32, EntryPoint = "GetWindowTextW", SetLastError = true)]
    public static unsafe partial int GetWindowText(nint hWnd, char* lpString, int nMaxCount);

    [LibraryImport(User32, EntryPoint = "SetWindowTextW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial int SetWindowText(nint hWnd, string title);

    [LibraryImport(User32, SetLastError = true)]
    public static partial int GetWindowRect(nint hWnd, ref RECT lpRect);

    [LibraryImport(User32, SetLastError = true)]
    public static partial int SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [LibraryImport(User32, SetLastError = true)]
    public static partial int GetSystemMetrics(int nIndex);

    [LibraryImport(User32, EntryPoint = "GetWindowLongW", SetLastError = true)]
    public static partial int GetWindowLong(nint hWnd, int nIndex);

    [LibraryImport(User32, EntryPoint = "SetWindowLongW", SetLastError = true)]
    public static partial int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [LibraryImport(User32, SetLastError = true)]
    public static partial int SetForegroundWindow(nint hWnd);

    [LibraryImport(User32, EntryPoint = "MessageBoxW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial int MessageBox(nint hWnd, string text, string caption, uint type);

    [LibraryImport(User32, SetLastError = true)]
    public static partial short GetAsyncKeyState(int vKey);
}