namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;
using ConsoleNexusEngine.Common;

internal static partial class Native
{
    private const int STD_OUTPUT_HANDLE = -11;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

    private const string RESET = "\x1B[0m";
    private const string UNDERLINE = "\x1B[4m";
    private const string BOLD = "\x1B[1m";
    private const string ITALIC = "\x1B[3m";
    private const string BLINK = "\x1B[5m";
    private const string BLINKRAPID = "\x1B[6m";
    private const string DEFAULTFORE = "\x1B[39m";
    private const string DEFAULTBACK = "\x1B[49m";

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial nint GetStdHandle(int nStdHandle);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

    static Native()
    {
        var handle = GetStdHandle(STD_OUTPUT_HANDLE);
        GetConsoleMode(handle, out var mode);
        mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        SetConsoleMode(handle, mode);
    }

    public static string ForeColor(string text, Color color)
        => $"\x1B[38;2;{color.R};{color.G};{color.B}m{text}";

    public static string BackColor(string text, Color color)
        => $"\x1B[48;2;{color.R};{color.G};{color.B}m{text}";

    public static string ResetColor(string text)
        => $"{RESET}{text}";

    public static string Bold(string text)
        => $"{BOLD}{text}";

    public static string Italic(string text)
        => $"{ITALIC}{text}";

    public static string Underline(string text)
        => $"{UNDERLINE}{text}";
}