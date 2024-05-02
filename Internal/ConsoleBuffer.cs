namespace ConsoleNexusEngine.Internal;

internal sealed unsafe class ConsoleBuffer
{
    private readonly nint _fileHandle;

    private CHAR_INFO[] charInfoBuffer;

    public short Width { get; private set; }
    public short Height { get; private set; }

    public event EventHandler? Updated;

    public ConsoleBuffer(in int width, in int height)
    {
        Width = (short)width;
        Height = (short)height;

        fixed (char* fileNameP = "CONOUT$")
        {
            _fileHandle = Native.CreateFile(fileNameP, 0x40000000, 2, nint.Zero, 3, 0, nint.Zero);
        }
        
        if (_fileHandle == nint.Zero) throw new NexusEngineException("The file handle for the console buffer is invalid");

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
        var attributes = (short)(background | background << 4);

        for (int i = 0; i < charInfoBuffer.Length; i++)
        {
            charInfoBuffer[i].Attributes = attributes;
            charInfoBuffer[i].UnicodeChar = char.MinValue;
        }
    }

    public void SetBackgroundBuffer(in Memory2D<Glyph> glyphBuffer, in int background)
    {
        Glyph current;

        for (int i = 0; i < glyphBuffer.Length; i++)
        {
            current = glyphBuffer[i];

            charInfoBuffer[i].Attributes =
                    (short)(current.ForegroundIndex | (current.Value == char.MinValue ? background << 4 : current.BackgroundIndex << 4));
            charInfoBuffer[i].UnicodeChar = current.Value;
        }
    }

    public void SetBuffer(in int x, in int y, in Glyph glyph)
    {
        var index = y * Width + x;

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

        fixed (CHAR_INFO* arrayP = &charInfoBuffer[0])
        {
            Native.WriteConsoleOutputW(
                _fileHandle,
                arrayP,
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
                &rect);
        }
    }
}