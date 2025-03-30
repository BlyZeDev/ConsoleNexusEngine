namespace ConsoleNexusEngine.Internal;

using System.Runtime.CompilerServices;

internal sealed unsafe class ConsoleBuffer
{
    private static readonly COORD _defaultBufferCoord = new COORD
    {
        X = 0,
        Y = 0
    };

    private readonly nint _standardOutput;

    private CHARINFO[] charInfoBuffer;

    private COORD bufferSize;

    public short Width { get; private set; }
    public short Height { get; private set; }

    public ConsoleBuffer(in nint standardOutput, in int width, in int height)
    {
        _standardOutput = standardOutput;

        Width = (short)width;
        Height = (short)height;

        charInfoBuffer = new CHARINFO[Width * Height];

        bufferSize = new COORD
        {
            X = Width,
            Y = Height
        };
    }

    public void ChangeDimensions(in int width, in int height)
    {
        Width = (short)width;
        Height = (short)height;

        Array.Resize(ref charInfoBuffer, Width * Height);

        bufferSize = new COORD
        {
            X = Width,
            Y = Height
        };
    }

    public void ClearChar(in int x, in int y)
    {
        fixed (CHARINFO* ptr = &charInfoBuffer[IndexDimensions.Get1D(x, y, Width)])
        {
            Unsafe.InitBlockUnaligned(ptr, 0, (uint)sizeof(CHARINFO));
        }
    }

    public void BlockClearChar(in int startX, in int startY, in int endX, in int endY)
    {
        fixed (CHARINFO* ptr = &charInfoBuffer[IndexDimensions.Get1D(startX, startY, Width)])
        {
            Unsafe.InitBlockUnaligned(ptr, 0, (uint)(IndexDimensions.Get1D(endX, endY, Width) * sizeof(CHARINFO)));
        }
    }

    public void ClearBuffer()
    {
        fixed (CHARINFO* ptr = &charInfoBuffer[0])
        {
            Unsafe.InitBlockUnaligned(ptr, 0, (uint)(charInfoBuffer.Length * sizeof(CHARINFO)));
        }
    }

    public CHARINFO GetChar(in int x, in int y) => charInfoBuffer[IndexDimensions.Get1D(x, y, Width)];

    public void SetChar(in int x, in int y, CHARINFO character)
    {
        ref var current = ref charInfoBuffer[IndexDimensions.Get1D(x, y, Width)];

        if (current.UnicodeChar == character.UnicodeChar
            && current.Attributes == character.Attributes) return;

        current = character;
    }

    public void BlockSetChar(in ReadOnlySpan<CHARINFO> characterBlock, in int sourceIndex, in int destIndex, in int blockWidth)
        => characterBlock.Slice(sourceIndex, blockWidth).CopyTo(charInfoBuffer.AsSpan(destIndex, blockWidth));

    public void RenderBuffer()
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
            Native.WriteConsoleOutput(
                _standardOutput,
                arrayPtr,
                bufferSize,
                _defaultBufferCoord,
                &renderArea);
        }
    }
}