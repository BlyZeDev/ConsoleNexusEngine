namespace ConsoleNexusEngine.Internal;

using System;

internal readonly struct CmdConsole
{
    public nint Handle { get; }
    public nint StandardInput { get; }
    public nint StandardOutput { get; }
    public ConsoleBuffer Buffer { get; }

    public int Width { get; }
    public int Height { get; }

    public CmdConsole(int fontWidth, int fontHeight)
    {
        Handle = Native.GetConsoleHandle();
        StandardInput = Native.GetConsoleStdInput();
        StandardOutput = Native.GetConsoleStdOutput();

        Console.CursorVisible = false;

        Native.SetConsoleFont(StandardOutput, fontWidth, fontHeight);
        (Width, Height) = Native.SetConsoleBorderless(Handle, StandardOutput, fontWidth, fontHeight);
        Native.FocusConsoleWindow(Handle);

        Buffer = new ConsoleBuffer(Width, Height);

        Native.SetConsoleMode(StandardInput, 0x0080);
        Native.SetConsoleMode(StandardInput, Native.GetConsoleMode(StandardInput) | 0x4);
    }
}