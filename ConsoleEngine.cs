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

    internal ColorPalette ColorPalette => _console.ColorPalette;
    internal int Width => _console.Width;
    internal int Height => _console.Height;

    internal int Background { get; private set; }

    internal ConsoleEngine(int fontWidth, int fontHeight, ColorPalette colorPalette)
    {
        _console = new CmdConsole(fontWidth, fontHeight, colorPalette);

        charBuffer = new NexusChar[_console.Width, _console.Height];

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
        character.foregroundColorIndex = GetColorIndex(character.Foreground);
        character.backgroundColorIndex = GetColorIndex(character.Background);

        if (character.foregroundColorIndex is -1 || character.backgroundColorIndex is - 1)
            throw new ArgumentException("The color is not in the color palette", nameof(character));

        if (!charBuffer.IsInRange(coordinate))
            throw new ArgumentOutOfRangeException(nameof(coordinate), "The coordinate is not in bounds of the console buffer");

        SetNexusChar(coordinate, character);
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
        _console.Buffer.RenderBuffer();
    }

    private void SetNexusChar(Coord coord, NexusChar nexusChar)
        => charBuffer[coord.X, coord.Y] = nexusChar;

    private int GetColorIndex(NexusColor color)
        => ColorPalette.Colors.GetKey(color);
}