namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal;
using System;

/// <summary>
/// The core engine of the <see cref="ConsoleGame"/>
/// </summary>
public sealed class ConsoleEngine
{
    private readonly CmdConsole _console;

    private NexusChar[,] charBuffer;

    /// <summary>
    /// The background color of the whole Console Window
    /// </summary>
    public NexusColor Background { get; set; }

    internal ConsoleEngine(int fontWidth, int fontHeight)
    {
        _console = new CmdConsole(fontWidth, fontHeight);

        charBuffer = new NexusChar[_console.Width, _console.Height];
    }

    public void SetPixel(Coord coordinate, NexusChar character)
    {
        SetNexusChar(coordinate, character);
    }

    /// <summary>
    /// Clears the current content of the console
    /// </summary>
    public void Clear()
        => Array.Clear(charBuffer);

    /// <summary>
    /// Renders changes to the console<br/>
    /// Use this method as last call in <see cref="ConsoleGame.Render"/>
    /// </summary>
    public void Render()
    {
        _console.Buffer.SetBuffer(charBuffer, Background);
        _console.Buffer.Blit();
    }

    private void SetNexusChar(Coord coord, NexusChar glyph)
        => charBuffer[coord.X, coord.Y] = glyph;
}