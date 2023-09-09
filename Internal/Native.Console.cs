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

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleScreenBufferSize(nint hConsoleOutput, COORD dwSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleScreenBufferInfoEx(nint hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleScreenBufferInfoEx(nint hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetWindowRect(nint hWnd, ref RECT lpRect);
    
    [LibraryImport("user32.dll")]
    private static partial nint GetDesktopWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);
        
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DrawMenuBar(nint hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int MapWindowPoints(nint hWndFrom, nint hWndTo, [In, Out] ref RECT rect, [MarshalAs(UnmanagedType.U4)] int cPoints);

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
    public static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            nint securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            nint template);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteConsoleOutputW(
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

    public static (int width, int height) SetConsoleBorderless(nint consoleHandle, nint stdOutput, in int fontWidth, in int fontHeight)
    {
        var consoleRect = new RECT();
        var desktopRect = new RECT();

        GetWindowRect(consoleHandle, ref consoleRect);
        var desktopHandle = GetDesktopWindow();
        _ = MapWindowPoints(desktopHandle, consoleHandle, ref consoleRect, 2);
        GetWindowRect(desktopHandle, ref desktopRect);

        _ = SetWindowLong(consoleHandle, -16, 0x00080000);

        var windowWidth = desktopRect.Right - desktopRect.Left;
        var windowHeight = desktopRect.Bottom - desktopRect.Top;

        SetConsoleScreenBufferSize(
            stdOutput,
            new COORD
            {
                X = (short)(windowWidth / fontWidth),
                Y = (short)(windowHeight / fontHeight)
            });

        SetWindowPos(
            consoleHandle,
            nint.Zero,
            0, 0,
            windowWidth,
            windowHeight,
            0x0040);

        DrawMenuBar(consoleHandle);

        return (windowWidth, windowHeight);
    }

    public static void FocusConsoleWindow(nint consoleHandle)
        => SetForegroundWindow(consoleHandle);

    public static uint GetConsoleMode(nint inputHandle)
    {
        GetConsoleMode(inputHandle, out var mode);
        return mode;
    }

    public static void SetConsoleMode(nint inputHandle, in uint mode)
        => SetConsoleMode(inputHandle, mode);

    public static void SetConsoleFont(nint stdOutput, in int fontWidth, in int fontHeight)
    {
        var font = new CONSOLE_FONT_INFO_EX();
        font.cbSize = (uint)Marshal.SizeOf(font);
        font.nFont = 0;

        var sizeX = (short)fontWidth;
        var sizeY = (short)fontHeight;

        font.dwFontSize.X = sizeX;
        font.dwFontSize.Y = sizeY;

        font.FaceName = sizeX < 4 || sizeY < 4 ? "Consolas" : "Terminal";

        SetCurrentConsoleFontEx(stdOutput, false, ref font);
    }

    public static SafeFileHandle CreateConOutFile()
        => CreateFile("CONOUT$", 0x40000000, 2, nint.Zero, FileMode.Open, 0, nint.Zero);

    public static void WriteToConsoleBuffer(SafeFileHandle handle, CHAR_INFO[] charBuffer, COORD consoleSize, ref SMALL_RECT region)
        => WriteConsoleOutputW(handle, charBuffer, consoleSize, new COORD { X = 0, Y = 0 }, ref region);

    public static void SetColorPalette(nint stdOutput, ColorPalette colorPalette)
    {
        var csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        csbe.cbSize = Marshal.SizeOf(csbe);

        GetConsoleScreenBufferInfoEx(stdOutput, ref csbe);

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

        ++csbe.srWindow.Bottom;
        ++csbe.srWindow.Right;

        SetConsoleScreenBufferInfoEx(stdOutput, ref csbe);
    }
}