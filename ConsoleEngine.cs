namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal;
using ConsoleNexusEngine.Internal.Models;
using System;

/// <summary>
/// The core engine of the <see cref="ConsoleGame"/>
/// </summary>
public sealed class ConsoleEngine
{
    private readonly CmdConsole _console;

    private Glyph[,] glyphBuffer;

    internal ColorPalette ColorPalette => _console.ColorPalette;
    internal int Width => _console.Width;
    internal int Height => _console.Height;

    internal int Background { get; private set; }

    internal ConsoleEngine(int fontWidth, int fontHeight, ColorPalette colorPalette)
    {
        _console = new CmdConsole(fontWidth, fontHeight, colorPalette);

        glyphBuffer = new Glyph[_console.Width, _console.Height];

        Background = 0;
    }

    /// <summary>
    /// Set a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="character"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetPixel(Coord coordinate, NexusChar character)
    {
        var foregroundColorIndex = GetColorIndex(character.Foreground);
        var backgroundColorIndex = GetColorIndex(character.Background);

        if (foregroundColorIndex is -1 || backgroundColorIndex is - 1)
            throw new ArgumentException("The color is not in the color palette", nameof(character));

        if (!glyphBuffer.IsInRange(coordinate))
            throw new ArgumentOutOfRangeException(nameof(coordinate), "The coordinate is not in bounds of the console buffer");

        SetGlyph(coordinate, new Glyph(character.Value, foregroundColorIndex, backgroundColorIndex));
    }

    /// <summary>
    /// Set the background of the whole console to a specific color
    /// </summary>
    /// <param name="color">The color to set as background</param>
    /// <exception cref="ArgumentException"></exception>
    public void SetBackground(NexusColor color)
    {
        var index = ColorPalette.GetIndex(color);

        if (index is -1)
            throw new ArgumentException("The color is not in the color palette", nameof(color));

        Background = index;
    }

    /// <summary>
    /// Clears the current content of the console<br/>
    /// Remember to call <see cref="ConsoleGame.Render"/>
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
        _console.Buffer.RenderBuffer();
    }

    private void SetGlyph(in Coord coord, in Glyph glyph)
        => glyphBuffer[coord.X, coord.Y] = glyph;

    private int GetColorIndex(in NexusColor color)
        => ColorPalette.Colors.GetKey(color);
}