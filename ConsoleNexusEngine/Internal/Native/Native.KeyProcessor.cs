﻿namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal static unsafe partial class Native
{
    [LibraryImport(User32)]
    public static partial short GetAsyncKeyState(int vKey);

    [LibraryImport(Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetNumberOfConsoleInputEvents(nint hConsoleInput, out int lpNumberOfEvents);

    [DllImport(Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PeekConsoleInput(nint hConsoleInput, [Out] INPUT_RECORD* lpBuffer, int nLength, out int lpNumberOfEventsRead);

    [LibraryImport(Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FlushConsoleInputBuffer(nint hConsoleInput);
}