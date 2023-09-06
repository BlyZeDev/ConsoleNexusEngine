namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal;
using ConsoleNexusEngine.Internal.Models;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// The core engine of the <see cref="ConsoleGame"/>
/// </summary>
public sealed class ConsoleEngine
{
    private readonly CmdConsole _console;

    private Glyph[,] glyphBuffer;

    /// <summary>
    /// The background color of the whole Console Window
    /// </summary>
    public NexusColor Background { get; set; }

    internal ConsoleEngine(int width, int height)
    {
        _console = new CmdConsole(width, height);

        glyphBuffer = new Glyph[width, height];

        Native.SetConsoleMode(_console.StandardInput, 0x0080);
    }

    /// <summary>
    /// Set the font of the Console
    /// </summary>
    /// <param name="font">The font to set</param>
    public unsafe void SetFont(GameFont font)
    {
        var fontInfo = new CONSOLE_FONT_INFO_EX
        {
            cbSize = Marshal.SizeOf<CONSOLE_FONT_INFO_EX>(),
            nFont = 0,
            FontWidth = 0,
            FontHeight = (short)font.Size,
            FontWeight = font.Weight
        };

        var name = font.Name.AsSpan();
        for (int i = 0; i < name.Length; i++)
        {
            fontInfo.FaceName[i] = name[i];
        }

        Native.SetConsoleFont(_console.StandardOutput, ref fontInfo);
    }

    public void SetPixel(Coord coordinate, char pixel)
    {
        SetGlyph(coordinate, new Glyph(pixel, NexusColor.Red, NexusColor.Green));
    }

    /// <summary>
    /// Clears the current content of the console
    /// </summary>
    public void Clear()
        => Array.Clear(glyphBuffer);

    /// <summary>
    /// Renders changes to the console<br/>
    /// Use this method as last call in <see cref="ConsoleGame.Render"/>
    /// </summary>
    public void Render()
    {
        _console.Buffer.SetBuffer(glyphBuffer, Background);
        _console.Buffer.Blit();
    }

    private void SetGlyph(Coord coord, Glyph glyph)
        => glyphBuffer[coord.X, coord.Y] = glyph;
}