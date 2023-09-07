﻿namespace ConsoleNexusEngine.Internal;

using System;

internal readonly struct CmdConsole
{
    public nint Handle { get; }
    public nint StandardInput { get; }
    public nint StandardOutput { get; }
    public ConsoleBuffer Buffer { get; }

    public int Width { get; }
    public int Height { get; }

    public CmdConsole() => throw new InvalidOperationException();

    public CmdConsole(int fontWidth, int fontHeight)
    {
        Handle = Native.GetConsoleHandle();
        StandardInput = Native.GetConsoleStdInput();
        StandardOutput = Native.GetConsoleStdOutput();

        Native.SetConsoleFont(StandardOutput, fontWidth, fontHeight);
        (Width, Height) = Native.SetConsoleBorderless(Handle, fontWidth, fontHeight);
        
        Buffer = new ConsoleBuffer(Width, Height);

        Native.SetConsoleMode(StandardInput, 0x0080);
    }
}