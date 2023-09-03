namespace ConsoleNexusEngine.Internal;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal.Models;
using System.Runtime.InteropServices;

internal static partial class Native
{
    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial nint GetStdHandle(int nStdHandle);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial nint GetConsoleWindow();

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial int GetCurrentConsoleFontEx(nint hConsoleOutput, [MarshalAs(UnmanagedType.Bool)] bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetCurrentConsoleFontEx(nint hConsoleOutput, [MarshalAs(UnmanagedType.Bool)] bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);

    public static nint GetConsoleStdOutput()
        => GetStdHandle(-11);

    public static bool SetConsoleFont(nint hConsoleOutput, ref CONSOLE_FONT_INFO_EX fontInfo)
        => SetCurrentConsoleFontEx(hConsoleOutput, false, ref fontInfo);

    public static string ForeColor(string text, GameColor color)
        => $"\x1B[38;2;{color.R};{color.G};{color.B}m{text}";

    public static string BackColor(string text, GameColor color)
        => $"\x1B[48;2;{color.R};{color.G};{color.B}m{text}";

    public static string ResetColor(string text)
        => $"\u001b[0m{text}";

    public static string BoldText(string text)
        => $"\u001b[1m{text}";

    public static string ItalicText(string text)
        => $"\u001b[3m{text}";

    public static string UnderlineText(string text)
        => $"\x1B[4m{text}";
}