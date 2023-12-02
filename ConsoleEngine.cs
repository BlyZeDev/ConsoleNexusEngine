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
    private Glyph[,] glyphBuffer;

    internal readonly CmdConsole _console;

    internal string Title => _console.Title;
    internal ColorPalette ColorPalette => _console.ColorPalette;
    internal int Width => _console.Width;
    internal int Height => _console.Height;
    internal NexusFont Font => _console.Font;

    internal int Background { get; private set; }

    internal ConsoleEngine(NexusFont font, ColorPalette colorPalette, string title)
    {
        _console = new CmdConsole(font, colorPalette, title);

        glyphBuffer = new Glyph[_console.Width, _console.Height];

        Background = 0;
    }

    /// <summary>
    /// Gets a pixel in the console at a specific position
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
    /// Gets the whole buffer of the console as 2d array
    /// </summary>
    /// <remarks>
    /// This includes characters that are not rendered yet
    /// </remarks>
    /// <returns><see cref="NexusChar"/>[,]</returns>
    public NexusChar[,] GetBuffer()
    {
        var buffer = new NexusChar[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                buffer[x, y] = NexusChar.FromGlyph(glyphBuffer[x, y], ColorPalette);
            }
        }

        return buffer;
    }

    /// <summary>
    /// Sets a pixel in the console at a specific position
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
    /// Sets a text in the console at a specific position
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
    /// Sets a text in the console at a specific position
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
    /// Sets a pixel in the console at specific positions
    /// </summary>
    /// <param name="character">The character itself</param>
    /// <param name="coordinates">The coordinates where the character should be placed</param>
    public void SetPixels(NexusChar character, params Coord[] coordinates)
    {
        foreach (var coordinate in coordinates)
        {
            SetPixel(coordinate, character);
        }
    }

    /// <summary>
    /// Draws a line from one coordinate to the other
    /// </summary>
    /// <param name="start">The coordinate of the start point</param>
    /// <param name="end">The coordinate of the end point</param>
    /// <param name="character">The character itself</param>
    public void DrawLine(Coord start, Coord end, NexusChar character)
    {
        var x = start.X;
        var y = start.Y;
        var x2 = end.X;
        var y2 = end.Y;

        var w = x2 - x;
        var h = y2 - y;

        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;

        if (w < 0) dx1 = -1;
        else if (w > 0) dx1 = 1;

        if (h < 0) dy1 = -1;
        else if (h > 0) dy1 = 1;

        if (w < 0) dx2 = -1;
        else if (w > 0) dx2 = 1;

        var longest = Math.Abs(w);
        var shortest = Math.Abs(h);

        if (!(longest > shortest))
        {
            longest = Math.Abs(h);
            shortest = Math.Abs(w);

            if (h < 0) dy2 = -1;
            else if (h > 0) dy2 = 1;

            dx2 = 0;
        }

        var numerator = longest >> 1;

        for (int i = 0; i <= longest; i++)
        {
            SetPixel(new Coord(x, y), character);

            numerator += shortest;

            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
    }

    /// <summary>
    /// Fills a space from one coordinate to the other
    /// </summary>
    /// <param name="start">The coordinate of the start point</param>
    /// <param name="end">The coordinate of the end point</param>
    /// <param name="character">The character itself</param>
    public void Fill(Coord start, Coord end, NexusChar character)
    {
        for (int sx = start.X; sx <= end.X; sx++)
        {
            for (int sy = start.Y; sy <= end.Y; sy++)
            {
                SetPixel(new Coord(sx, sy), character);
            }
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
    /// Clears a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be cleared</param>
    public void ClearAt(Coord coordinate)
    {
        ThrowIfOutOfBounds(coordinate);

        SetGlyph(coordinate, new Glyph('\0', Background, Background));
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