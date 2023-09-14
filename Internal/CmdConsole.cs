namespace ConsoleNexusEngine.Internal;

using ConsoleNexusEngine.Common;
using System;

internal readonly record struct CmdConsole
{
    public nint Handle { get; }
    public nint StandardInput { get; }
    public nint StandardOutput { get; }
    public ConsoleBuffer Buffer { get; }

    public int Width { get; }
    public int Height { get; }

    public int FontWidth { get; }
    public int FontHeight { get; }

    public ColorPalette ColorPalette { get; }

    public CmdConsole(int fontWidth, int fontHeight, ColorPalette colorPalette)
    {
        Handle = Native.GetConsoleHandle();
        StandardInput = Native.GetConsoleStdInput();
        StandardOutput = Native.GetConsoleStdOutput();

        Console.CursorVisible = false;

        ColorPalette = colorPalette;

        FontWidth = fontWidth;
        FontHeight = fontHeight;

        Native.SetConsoleFont(StandardOutput, fontWidth, fontHeight);
        Native.SetConsoleMode(StandardInput, 0x0080);

        var size = Native.InitializeConsole(Handle, StandardOutput, fontWidth, fontHeight, colorPalette);
        Width = size.X;
        Height = size.Y;

        Native.FocusConsoleWindow(Handle);

        Buffer = new ConsoleBuffer(Width, Height);
    }
}