namespace ConsoleNexusEngine.Internal;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal.Models;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

internal sealed class ConsoleBuffer
{
    private readonly int _width;
    private readonly int _height;
    private readonly SafeFileHandle _file;

    private CHAR_INFO[] charInfoBuffer;

    public ConsoleBuffer(in int width, in int height)
    {
        _width = width;
        _height = height;

        _file = Native.CreateConOutFile();

        if (_file.IsInvalid)
            throw new ExternalException("The SafeFileHandle for the Console Buffer was invalid");
        
        charInfoBuffer = new CHAR_INFO[_width * _height];
    }

    public void ClearBuffer(in int background)
    {
        int index;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                index = y * _width + x;

                charInfoBuffer[index].Attributes = (short)(background | background << 4);
                charInfoBuffer[index].UnicodeChar = (char)0;
            }
        }
    }

    public void SetBackgroundBuffer(ref Glyph[,] glyphBuffer, in int background)
    {
        Glyph current;
        int index;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                index = y * _width + x;
                current = glyphBuffer[x, y];

                charInfoBuffer[index].Attributes =
                    (short)(current.ForegroundIndex | (current.Value == 0 ? background << 4 : current.BackgroundIndex << 4));
                charInfoBuffer[index].UnicodeChar = current.Value;
            }
        }
    }

    public void SetBuffer(in Coord coord, in Glyph glyph)
    {
        var index = coord.Y * _width + coord.X;

        charInfoBuffer[index].Attributes = (short)(glyph.ForegroundIndex | glyph.BackgroundIndex << 4);
        charInfoBuffer[index].UnicodeChar = glyph.Value;
    }

    public void RenderBuffer()
    {
        var rect = new SMALL_RECT
        {
            Left = 0,
            Top = 0,
            Right = (short)_width,
            Bottom = (short)_height
        };

        Native.WriteToConsoleBuffer(_file, charInfoBuffer, new COORD { X = (short)_width, Y = (short)_height }, ref rect);
    }
}