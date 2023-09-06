namespace ConsoleNexusEngine.Internal;

using System;

internal readonly struct CmdConsole
{
    public nint Handle { get; }
    public nint StandardInput { get; }
    public nint StandardOutput { get; }
    public ConsoleBuffer Buffer { get; }

    public CmdConsole() => throw new InvalidOperationException();

    public CmdConsole(int width, int height)
    {
        Handle = Native.GetConsoleHandle();
        StandardInput = Native.GetConsoleStdInput();
        StandardOutput = Native.GetConsoleStdOutput();

        Buffer = new ConsoleBuffer(width, height);
    }
}