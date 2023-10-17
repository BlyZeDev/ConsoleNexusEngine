namespace ConsoleNexusEngine.Internal;

using ConsoleNexusEngine.Common;
using System;

internal sealed record CmdConsole
{
    public nint Handle { get; }
    public nint StandardInput { get; }
    public nint StandardOutput { get; }
    public ConsoleBuffer Buffer { get; }

    public string Title { get; }

    public int Width { get; }
    public int Height { get; }

    public NexusFont Font { get; }

    public ColorPalette ColorPalette { get; }

    public CmdConsole(NexusFont font, ColorPalette colorPalette, string title)
    {
        Handle = Native.GetConsoleHandle();
        StandardInput = Native.GetConsoleStdInput();
        StandardOutput = Native.GetConsoleStdOutput();

        Console.CursorVisible = false;

        Title = title;

        ColorPalette = colorPalette;

        Font = font;

        Native.SetConsoleFont(StandardOutput, font);
        Native.SetConsoleMode(StandardInput, 0x0080);

        var size = Native.InitializeConsole(Handle, StandardOutput, font.Width, font.Height, colorPalette, title);
        Width = size.X;
        Height = size.Y;

        Buffer = new ConsoleBuffer(Width, Height);
    }
}