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
    internal NexusFont Font => _console.Font;

    internal int Background { get; private set; }

    internal ConsoleEngine(NexusFont font, ColorPalette colorPalette)
    {
        _console = new CmdConsole(font, colorPalette);

        glyphBuffer = new Glyph[_console.Width, _console.Height];

        Background = 0;
    }

    /// <summary>
    /// Get a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be placed</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns><see cref="NexusChar"/></returns>
    public NexusChar GetPixel(Coord coordinate)
    {
        ThrowIfOutOfBounds(coordinate);

        var glyph = glyphBuffer[coordinate.X, coordinate.Y];

        return new(glyph.Value, ColorPalette[glyph.ForegroundIndex], ColorPalette[glyph.BackgroundIndex]);
    }

    /// <summary>
    /// Set a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be placed</param>
    /// <param name="character">The character itself</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetPixel(Coord coordinate, NexusChar character)
    {
        ThrowIfOutOfBounds(coordinate);

        var foregroundColorIndex = GetColorIndex(character.Foreground);
        var backgroundColorIndex = character.Background is null
            ? Background : GetColorIndex(character.Background.Value);

        if (foregroundColorIndex is -1 || backgroundColorIndex is -1)
            throw new ArgumentException("The color is not in the color palette", nameof(character));

        SetGlyph(coordinate, new Glyph(character.Value, foregroundColorIndex, backgroundColorIndex));
    }

    /// <summary>
    /// Set a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the text should start</param>
    /// <param name="text">The text itself</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetText(Coord coordinate, NexusText text)
    {
        var isHorizontal = text.TextDirection is TextDirection.Horizontal;
        ThrowIfOutOfBounds(coordinate + (isHorizontal ? new Coord(text.Value.Length - 1, 0) : new Coord(0, text.Value.Length - 1)));

        var foregroundColorIndex = GetColorIndex(text.Foreground);
        var backgroundColorIndex = text.Background is null
            ? Background : GetColorIndex(text.Background.Value);
        
        if (foregroundColorIndex is -1 || backgroundColorIndex is -1)
            throw new ArgumentException("The color is not in the color palette", nameof(text));

        var posX = -1;
        var posY = -1;
        foreach (var letter in text.Value)
        {
            if (isHorizontal) posX++;
            else posY++;

            SetGlyph(coordinate + new Coord(posX, posY), new Glyph(letter, foregroundColorIndex, backgroundColorIndex));
        }
    }

    /// <summary>
    /// Set a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the text should start</param>
    /// <param name="text">The text itself</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetText(Coord coordinate, NexusFiggleText text)
    {
        ThrowIfOutOfBounds(coordinate + new Coord(text._longestStringLength, text.Value.Length - 1));

        var foregroundColorIndex = GetColorIndex(text.Foreground);
        var backgroundColorIndex = text.Background is null
            ? Background : GetColorIndex(text.Background.Value);

        if (foregroundColorIndex is -1 || backgroundColorIndex is -1)
            throw new ArgumentException("The color is not in the color palette", nameof(text));

        var posX = -1;
        var posY = -1;
        foreach (var letters in text.Value)
        {
            posY++;
            foreach (var letter in letters)
            {
                posX++;
                SetGlyph(coordinate + new Coord(posX, posY), new Glyph(letter, foregroundColorIndex, backgroundColorIndex));
            }

            posX = -1;
        }
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

        _console.Buffer.SetBackgroundBuffer(ref glyphBuffer, Background);
    }

    /// <summary>
    /// Clears the current content of the console<br/>
    /// Remember to call <see cref="ConsoleGame.Render"/>
    /// </summary>
    public void Clear()
    {
        Array.Clear(glyphBuffer);
        _console.Buffer.ClearBuffer(Background);
    }

    /// <summary>
    /// Renders changes to the console<br/>
    /// Use this method as last call in <see cref="ConsoleGame.Render"/>
    /// </summary>
    public void Render()
        => _console.Buffer.RenderBuffer();

    private void SetGlyph(in Coord coord, in Glyph glyph)
    {
        glyphBuffer[coord.X, coord.Y] = glyph;
        _console.Buffer.SetBuffer(coord, glyph);
    }

    private void ThrowIfOutOfBounds(in Coord coord)
    {
        if (!glyphBuffer.IsInRange(coord))
            throw new ArgumentOutOfRangeException(nameof(coord), "The coordinate is not in bounds of the console buffer");
    }

    private int GetColorIndex(in NexusColor color)
        => ColorPalette.Colors.GetKey(color);
}