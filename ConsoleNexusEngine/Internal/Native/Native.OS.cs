namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;
using System.Text;

internal static partial class Native
{
    [DllImport(User32)]
    public static extern nint MonitorFromWindow(nint hwnd, uint dwFlags);

    [DllImport(User32, CharSet = CharSet.Auto)]
    public static extern bool GetMonitorInfo(nint hMonitor, ref MONITORINFO lpmi);

    [LibraryImport(User32)]
    public static partial int ShowWindow(nint hWnd, int nCmdShow);

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

    [LibraryImport(User32)]
    public static partial int GetSystemMetrics(int nIndex);

    [DllImport(User32)]
    public static extern int GetWindowLong(nint hWnd, int nIndex);

    [DllImport(User32)]
    public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetForegroundWindow(nint hWnd);

    [DllImport(User32, CharSet = CharSet.Unicode)]
    public static extern int MessageBox(nint hWnd, string text, string caption, uint type);
}