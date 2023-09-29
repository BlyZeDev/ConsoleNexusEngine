namespace ConsoleNexusEngine.Internal;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal.Models;
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

internal static partial class Native
{
    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial nint GetStdHandle(int nStdHandle);

    [LibraryImport("kernel32.dll")]
    private static partial nint GetConsoleWindow();

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleScreenBufferInfoEx(nint hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleScreenBufferInfoEx(nint hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetWindowRect(nint hWnd, ref RECT lpRect);

    [LibraryImport("user32.dll")]
    private static partial int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);
        
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DrawMenuBar(nint hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetForegroundWindow(nint hWnd);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetCurrentConsoleFontEx(nint hConsoleOutput, [MarshalAs(UnmanagedType.Bool)] bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            nint securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            nint template);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteConsoleOutputW(
        SafeFileHandle hConsoleOutput,
        CHAR_INFO[] lpBuffer,
        COORD dwBufferSize,
        COORD dwBufferCoord,
        ref SMALL_RECT lpWriteRegion);

    public static nint GetConsoleHandle()
        => GetConsoleWindow();

    public static nint GetConsoleStdInput()
        => GetStdHandle(-10);

    public static nint GetConsoleStdOutput()
        => GetStdHandle(-11);

    public static void FocusConsoleWindow(in nint consoleHandle)
        => SetForegroundWindow(consoleHandle);

    public static uint GetConsoleMode(in nint inputHandle)
    {
        GetConsoleMode(inputHandle, out var mode);
        return mode;
    }

    public static void SetConsoleMode(in nint inputHandle, in uint mode)
        => SetConsoleMode(inputHandle, mode);

    public static void SetConsoleFont(in nint stdOutput, NexusFont font)
    {
        var fontInfo = new CONSOLE_FONT_INFO_EX();
        fontInfo.cbSize = (uint)Marshal.SizeOf(fontInfo);
        fontInfo.nFont = 0;

        fontInfo.dwFontSize.X = (short)font.Width;
        fontInfo.dwFontSize.Y = (short)font.Height;
        fontInfo.FaceName = font.Name;

        SetCurrentConsoleFontEx(stdOutput, false, ref fontInfo);
    }

    public static SafeFileHandle CreateConOutFile()
        => CreateFile("CONOUT$", 0x40000000, 2, nint.Zero, FileMode.Open, 0, nint.Zero);

    public static void WriteToConsoleBuffer(SafeFileHandle handle, CHAR_INFO[] charBuffer, COORD consoleSize, ref SMALL_RECT region)
        => WriteConsoleOutputW(handle, charBuffer, consoleSize, new COORD { X = 0, Y = 0 }, ref region);

    public static Coord InitializeConsole(in nint consoleHandle, in nint stdOutput, in int fontWidth, in int fontHeight, ColorPalette colorPalette)
    {
        var consoleRect = new RECT();
        GetWindowRect(consoleHandle, ref consoleRect);

        var desktopWidth = GetSystemMetrics(0);
        var desktopHeight = GetSystemMetrics(1);

        _ = SetWindowLong(consoleHandle, -16, 0x00080000);

        var csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        csbe.cbSize = Marshal.SizeOf(csbe);

        GetConsoleScreenBufferInfoEx(stdOutput, ref csbe);

        SetColorPalette(stdOutput, colorPalette, ref csbe);

        csbe.dwSize.X = 1;
        csbe.dwSize.Y = 1;
        ++csbe.srWindow.Bottom;
        ++csbe.srWindow.Right;

        SetConsoleScreenBufferInfoEx(stdOutput, ref csbe);

        SetWindowPos(
            consoleHandle,
            nint.Zero,
            0, 0,
            desktopWidth,
            desktopHeight,
            0x0040);
        
        DrawMenuBar(consoleHandle);

        GetConsoleScreenBufferInfoEx(stdOutput, ref csbe);

        return new(csbe.dwSize.X, csbe.dwSize.Y);
    }

    private static void SetColorPalette(in nint stdOutput, ColorPalette colorPalette, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe)
    {
        foreach (var color in colorPalette.Colors)
        {
            switch (color.Key)
            {
                case ConsoleColor.Black: csbe.black = new COLORREF(color.Value); break;
                case ConsoleColor.DarkBlue: csbe.darkBlue = new COLORREF(color.Value); break;
                case ConsoleColor.DarkGreen: csbe.darkGreen = new COLORREF(color.Value); break;
                case ConsoleColor.DarkCyan: csbe.darkCyan = new COLORREF(color.Value); break;
                case ConsoleColor.DarkRed: csbe.darkRed = new COLORREF(color.Value); break;
                case ConsoleColor.DarkMagenta: csbe.darkMagenta = new COLORREF(color.Value); break;
                case ConsoleColor.DarkYellow: csbe.darkYellow = new COLORREF(color.Value); break;
                case ConsoleColor.Gray: csbe.gray = new COLORREF(color.Value); break;
                case ConsoleColor.DarkGray: csbe.darkGray = new COLORREF(color.Value); break;
                case ConsoleColor.Blue: csbe.blue = new COLORREF(color.Value); break;
                case ConsoleColor.Green: csbe.green = new COLORREF(color.Value); break;
                case ConsoleColor.Cyan: csbe.cyan = new COLORREF(color.Value); break;
                case ConsoleColor.Red: csbe.red = new COLORREF(color.Value); break;
                case ConsoleColor.Magenta: csbe.magenta = new COLORREF(color.Value); break;
                case ConsoleColor.Yellow: csbe.yellow = new COLORREF(color.Value); break;
                case ConsoleColor.White: csbe.white = new COLORREF(color.Value); break;
            }
        }
    }
}