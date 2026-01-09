namespace ConsoleNexusEngine.Internal;

internal sealed class ConsoleBuffer
{
    private static readonly COORD _defaultBufferCoord = new COORD
    {
        X = 0,
        Y = 0
    };

    private readonly nint _standardOutput;

    private CHARINFO[] charInfoBuffer;
    private COORD bufferSize;

    public short Width => bufferSize.X;
    public short Height => bufferSize.Y;

    public ConsoleBuffer(nint standardOutput, int width, int height)
    {
        _standardOutput = standardOutput;

        bufferSize = new COORD
        {
            X = (short)width,
            Y = (short)height
        };

        charInfoBuffer = new CHARINFO[Width * Height];
    }

    public void ChangeDimensions(int width, int height)
    {
        bufferSize.X = (short)width;
        bufferSize.Y = (short)height;

        Array.Resize(ref charInfoBuffer, Width * Height);
    }

    public void ClearChar(int x, int y) => Array.Clear(charInfoBuffer, IndexDimensions.Get1D(x, y, Width), 1);

    public void ClearBuffer() => Array.Clear(charInfoBuffer);

    public CHARINFO GetChar(int x, int y) => charInfoBuffer[IndexDimensions.Get1D(x, y, Width)];

    public void SetChar(int x, int y, CHARINFO character)
    {
        ref var current = ref charInfoBuffer[IndexDimensions.Get1D(x, y, Width)];

        if (current.UnicodeChar == character.UnicodeChar
            && current.Attributes == character.Attributes) return;

        current = character;
    }

    public void BlockSetChar(in ReadOnlySpan<CHARINFO> characterBlock, int sourceIndex, int destIndex, int blockWidth)
        => characterBlock.Slice(sourceIndex, blockWidth).CopyTo(charInfoBuffer.AsSpan(destIndex, blockWidth));

    public unsafe void RenderBuffer()
    {
        var renderArea = new SMALL_RECT
        {
            Left = 0,
            Top = 0,
            Right = Width,
            Bottom = Height
        };

        fixed (CHARINFO* arrayPtr = &charInfoBuffer[0])
        {
            PInvoke.WriteConsoleOutput(
                _standardOutput,
                arrayPtr,
                bufferSize,
                _defaultBufferCoord,
                &renderArea);
        }
    }
}