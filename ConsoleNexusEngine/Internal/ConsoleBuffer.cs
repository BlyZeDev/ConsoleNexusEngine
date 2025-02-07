namespace ConsoleNexusEngine.Internal;

internal sealed unsafe class ConsoleBuffer
{
    private readonly nint _standardOutput;

    private CHAR_INFO[] charInfoBuffer;

    private bool needsRender;

    public short Width { get; private set; }
    public short Height { get; private set; }

    public event EventHandler? Updated;

    public ConsoleBuffer(in nint standardOutput, in int width, in int height)
    {
        _standardOutput = standardOutput;

        Width = (short)width;
        Height = (short)height;

        charInfoBuffer = new CHAR_INFO[Width * Height];
        needsRender = false;
    }

    public void ChangeDimensions(in int width, in int height)
    {
        Width = (short)width;
        Height = (short)height;

        Array.Resize(ref charInfoBuffer, Width * Height);

        Updated?.Invoke(this, EventArgs.Empty);

        needsRender = true;
    }

    public void ClearBuffer(in int background)
    {
        var attributes = (short)(background | background << 4);

        for (int i = 0; i < charInfoBuffer.Length; i++)
        {
            charInfoBuffer[i].Attributes = attributes;
            charInfoBuffer[i].UnicodeChar = char.MinValue;
        }

        needsRender = true;
    }

    public void SetBackgroundBuffer(in Memory2D<NexusChar> charBuffer, in int background)
    {
        var glyphs = charBuffer.Span;
        var shiftedBg = background << 4;

        for (int i = 0; i < glyphs.Length; i++)
        {
            ref var current = ref glyphs[i];

            charInfoBuffer[i].Attributes =
                    (short)(current.Foreground | (current.Value is char.MinValue ? shiftedBg : current.Background << 4));
            charInfoBuffer[i].UnicodeChar = current.Value;
        }

        needsRender = true;
    }

    public void SetChar(in int x, in int y, in NexusChar character)
    {
        var index = MathHelper.GetIndex(x, y, Width);

        var before = charInfoBuffer[index];
        var attributes = (short)(character.Foreground | character.Background << 4);

        if (before.UnicodeChar == character.Value && before.Attributes == attributes) return;

        needsRender = true;
        charInfoBuffer[index].Attributes = attributes;
        charInfoBuffer[index].UnicodeChar = character.Value;
    }

    public void RenderBuffer()
    {
        if (!needsRender) return;

        needsRender = false;

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
                _standardOutput,
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