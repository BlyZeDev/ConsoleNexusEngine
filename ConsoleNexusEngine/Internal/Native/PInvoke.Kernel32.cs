namespace ConsoleNexusEngine.Internal.Native;

using System.Runtime.InteropServices;

internal static partial class PInvoke
{
    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial int QueryPerformanceCounter(out long lpPerformanceCount);

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial int QueryPerformanceFrequency(out long lpFrequency);

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial int GetNumberOfConsoleInputEvents(nint hConsoleInput, out int lpNumberOfEvents);

    [LibraryImport(Kernel32, EntryPoint = "ReadConsoleInputExW", SetLastError = true)]
    public static unsafe partial int ReadConsoleInputEx(nint hConsoleInput, INPUT_RECORD* lpBuffer, int nLength, out int lpNumberOfEventsRead, uint flags);

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial int AllocConsole();

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial int FreeConsole();

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial nint GetStdHandle(int nStdHandle);

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial nint GetConsoleWindow();

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial int GetConsoleCursorInfo(nint hConsoleOutput, out CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial int SetConsoleCursorInfo(nint hConsoleOutput, ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial int GetConsoleScreenBufferInfoEx(nint hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial int SetConsoleScreenBufferInfoEx(nint hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial int GetConsoleMode(nint hConsoleHandle, out uint dwMode);

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial int SetConsoleMode(nint hConsoleHandle, uint dwMode);

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial int GetCurrentConsoleFontEx(nint hConsoleOutput, int bMaximumWindow, out CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);

    [LibraryImport(Kernel32, SetLastError = true)]
    public static partial int SetCurrentConsoleFontEx(nint hConsoleOutput, int bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);

    [LibraryImport(Kernel32, EntryPoint = "WriteConsoleOutputW", SetLastError = true)]
    public static unsafe partial int WriteConsoleOutput(nint hConsoleOutput, CHARINFO* lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, ref SMALL_RECT lpWriteRegion);
}