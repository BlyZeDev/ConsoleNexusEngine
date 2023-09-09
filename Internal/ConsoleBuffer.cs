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

    public ConsoleBuffer(int width, int height)
    {
        _width = width;
        _height = height;

        _file = Native.CreateConOutFile();

        if (_file.IsInvalid)
            throw new ExternalException("The SafeFileHandle for the Console Buffer was invalid");
        
        charInfoBuffer = new CHAR_INFO[_width * _height];
    }

    public void SetBuffer(NexusChar[,] charBuffer, int consoleBackground)
    {
        int index;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                index = (y * _width) + x;

                if (charBuffer[x, y].Value == 0)
                    charBuffer[x, y].backgroundColorIndex = consoleBackground;

                charInfoBuffer[index].Attributes = (short)(charBuffer[x, y].foregroundColorIndex | charBuffer[x, y].backgroundColorIndex << 4);
                charInfoBuffer[index].UnicodeChar = charBuffer[x, y].Value;
            }
        }
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

        Native.WriteToConsoleBuffer(
            _file, charInfoBuffer, new COORD { X = (short)_width, Y = (short)_height }, ref rect);
    }
}