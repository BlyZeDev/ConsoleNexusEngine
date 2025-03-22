namespace ConsoleNexusEngine.Internal;

using System.Runtime.CompilerServices;

internal sealed unsafe class ConsoleBuffer
{
    private static readonly COORD _defaultBufferCoord = new COORD
    {
        X = 0,
        Y = 0
    };
    private static readonly SMALL_RECT _defaultRenderArea = new SMALL_RECT
    {
        Left = short.MaxValue,
        Bottom = short.MinValue,
        Right = short.MinValue,
        Top = short.MaxValue
    };

    private readonly nint _standardOutput;

    private CHAR_INFO[] charInfoBuffer;

    private bool needsRender;
    private SMALL_RECT renderArea;
    private SMALL_RECT lastRendered;

    public short Width { get; private set; }
    public short Height { get; private set; }

    public ConsoleBuffer(in nint standardOutput, in int width, in int height)
    {
        _standardOutput = standardOutput;

        Width = (short)width;
        Height = (short)height;

        charInfoBuffer = new CHAR_INFO[Width * Height];
        needsRender = false;
        renderArea = new SMALL_RECT
        {
            Left = 0,
            Top = 0,
            Right = Width,
            Bottom = Height
        };
        lastRendered = renderArea;
    }

    public void ChangeDimensions(in int width, in int height)
    {
        Width = (short)width;
        Height = (short)height;

        Array.Resize(ref charInfoBuffer, Width * Height);

        SetRenderArea(0, 0, Width, Height);
    }

    public void ClearBuffer()
    {
        fixed (CHAR_INFO* ptr = &charInfoBuffer[0])
        {
            Unsafe.InitBlockUnaligned(ptr, 0, (uint)(charInfoBuffer.Length * sizeof(CHAR_INFO)));
        }

        SetRenderArea(lastRendered.Left, lastRendered.Top, lastRendered.Right, lastRendered.Bottom);
    }

    public CHAR_INFO GetChar(in int x, in int y) => charInfoBuffer[IndexDimensions.Get1D(x, y, Width)];

    public void SetChar(in int x, in int y, CHAR_INFO character)
    {
        ref var current = ref charInfoBuffer[IndexDimensions.Get1D(x, y, Width)];

        if (Unsafe.ReadUnaligned<long>(Unsafe.AsPointer(ref current)) == Unsafe.ReadUnaligned<long>(&character))
            return;

        current = character;
        SetRenderArea(x, y, x, y);
    }

    public void BlockSetChar(in ReadOnlySpan<CHAR_INFO> characterBlock, in int sourceIndex, in int destIndex, in int blockWidth)
    {
        characterBlock.Slice(sourceIndex, blockWidth).CopyTo(charInfoBuffer.AsSpan(destIndex, blockWidth));

        SetRenderArea(destIndex % Width, destIndex / Width, (destIndex + blockWidth - 1) % Width, (destIndex + blockWidth - 1) / Width);
    }

    public void RenderBuffer()
    {
        if (!needsRender) return;

        fixed (CHAR_INFO* arrayPtr = &charInfoBuffer[0])
        {
            Native.WriteConsoleOutput(
                _standardOutput,
                arrayPtr,
                new COORD
                {
                    X = Width,
                    Y = Height
                },
                _defaultBufferCoord,
                (SMALL_RECT*)Unsafe.AsPointer(ref renderArea));
        }

        lastRendered = renderArea;
        renderArea = _defaultRenderArea;
    }

    private void SetRenderArea(in int startX, in int startY, in int endX, in int endY)
    {
        renderArea.Left = (short)Math.Min(startX, renderArea.Left);
        renderArea.Top = (short)Math.Min(startY, renderArea.Top);
        renderArea.Right = (short)Math.Max(endX, renderArea.Right);
        renderArea.Bottom = (short)Math.Max(endY, renderArea.Bottom);
        needsRender = true;
    }
}