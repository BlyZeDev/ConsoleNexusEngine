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

    internal ConsoleEngine(int fontWidth, int fontHeight)
    {
        _console = new CmdConsole(fontWidth, fontHeight);

        glyphBuffer = new Glyph[_console.Width, _console.Height];
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