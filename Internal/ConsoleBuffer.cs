namespace ConsoleNexusEngine.Internal;

using Microsoft.Win32.SafeHandles;
using System.IO;

internal sealed class ConsoleBuffer
{
    private readonly SafeFileHandle _file;
    private readonly CHAR_INFO[] charInfoBuffer;

    public int Width { get; }

    public int Height { get; }

    public ConsoleBuffer(in int width, in int height)
    {
        Width = width;
        Height = height;

        _file = Native.CreateFile("CONOUT$", 0x40000000, 2, nint.Zero, FileMode.Open, 0, nint.Zero);

        if (_file.IsInvalid) throw new NexusEngineException("The SafeFileHandle for the Console Buffer is invalid");
        
        charInfoBuffer = new CHAR_INFO[Width * Height];
    }

    public void ClearBuffer(in int background)
    {
        int index;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                index = y * Width + x;

                charInfoBuffer[index].Attributes = (short)(background | background << 4);
                charInfoBuffer[index].UnicodeChar = (char)0;
            }
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
            Right = (short)Width,
            Bottom = (short)Height
        };

        Native.WriteConsoleOutputW(
            _file,
            charInfoBuffer,
            new COORD
            {
                X = (short)Width,
                Y = (short)Height
            },
            new COORD
            {
                X = 0,
                Y = 0
            },
            ref rect);
    }
}