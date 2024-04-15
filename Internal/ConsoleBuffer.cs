﻿namespace ConsoleNexusEngine.Internal;

using Microsoft.Win32.SafeHandles;
using System.IO;

internal sealed class ConsoleBuffer
{
    private readonly SafeFileHandle _file;

    private CHAR_INFO[] charInfoBuffer;

    public short Width { get; private set; }
    public short Height { get; private set; }

    public event EventHandler? Updated;

    public ConsoleBuffer(in int width, in int height)
    {
        Width = (short)width;
        Height = (short)height;

        _file = Native.CreateFile("CONOUT$", 0x40000000, 2, nint.Zero, FileMode.Open, 0, nint.Zero);

        if (_file.IsInvalid) throw new NexusEngineException("The SafeFileHandle for the Console Buffer is invalid");
        
        charInfoBuffer = new CHAR_INFO[Width * Height];
    }

    public void ChangeDimensions(in int width, in int height)
    {
        Width = (short)width;
        Height = (short)height;

        Array.Resize(ref charInfoBuffer, Width * Height);

        Updated?.Invoke(this, EventArgs.Empty);
    }

    public void ClearBuffer(in int background)
    {
        for (int i = 0; i < charInfoBuffer.Length; i++)
        {
            charInfoBuffer[i].Attributes = (short)(background | background << 4);
            charInfoBuffer[i].UnicodeChar = (char)0;
        }
    }

    public void SetBackgroundBuffer(ref Glyph[,] glyphBuffer, in int background)
    {
        Glyph current;
        int index;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                index = y * Width + x;
                current = glyphBuffer[x, y];

                charInfoBuffer[index].Attributes =
                    (short)(current.ForegroundIndex | (current.Value == 0 ? background << 4 : current.BackgroundIndex << 4));
                charInfoBuffer[index].UnicodeChar = current.Value;
            }
        }
    }

    public void SetBuffer(in Coord coord, in Glyph glyph)
    {
        var index = coord.Y * Width + coord.X;

        charInfoBuffer[index].Attributes = (short)(glyph.ForegroundIndex | glyph.BackgroundIndex << 4);
        charInfoBuffer[index].UnicodeChar = glyph.Value;
    }

    public void RenderBuffer()
    {
        var rect = new SMALL_RECT
        {
            Left = 0,
            Top = 0,
            Right = Width,
            Bottom = Height
        };
        
        Native.WriteConsoleOutputW(
            _file,
            charInfoBuffer,
            new COORD
            {
                X = Width,
                Y = Height
            },
            new COORD
            {
                X = 0,
                Y = 0
            },
            ref rect);
    }
}