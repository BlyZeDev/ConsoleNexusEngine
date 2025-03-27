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

    private CHARINFO[] charInfoBuffer;

    private bool needsRender;
    private SMALL_RECT renderArea;

    public short Width { get; private set; }
    public short Height { get; private set; }

    public ConsoleBuffer(in nint standardOutput, in int width, in int height)
    {
        _standardOutput = standardOutput;

        Width = (short)width;
        Height = (short)height;

        charInfoBuffer = new CHARINFO[Width * Height];
        needsRender = false;
        renderArea = new SMALL_RECT
        {
            Left = 0,
            Top = 0,
            Right = Width,
            Bottom = Height
        };
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
        fixed (CHARINFO* ptr = &charInfoBuffer[0])
        {
            Unsafe.InitBlockUnaligned(ptr, 0, (uint)(charInfoBuffer.Length * sizeof(CHARINFO)));
        }

        needsRender = true;
    }

    public CHARINFO GetChar(in int x, in int y) => charInfoBuffer[IndexDimensions.Get1D(x, y, Width)];

    public void SetChar(in int x, in int y, CHARINFO character)
    {
        ref var current = ref charInfoBuffer[IndexDimensions.Get1D(x, y, Width)];

        if (Unsafe.ReadUnaligned<long>(Unsafe.AsPointer(ref current)) == Unsafe.ReadUnaligned<long>(&character))
            return;

        current = character;
        SetRenderArea(x, y, x, y);
    }

    public void BlockSetChar(in ReadOnlySpan<CHARINFO> characterBlock, in int sourceIndex, in int destIndex, in int blockWidth)
        => characterBlock.Slice(sourceIndex, blockWidth).CopyTo(charInfoBuffer.AsSpan(destIndex, blockWidth));

    public void SetRenderArea(in int startX, in int startY, in int endX, in int endY)
    {
        renderArea.Left = (short)Math.Min(startX, renderArea.Left);
        renderArea.Top = (short)Math.Min(startY, renderArea.Top);
        renderArea.Right = (short)Math.Max(endX, renderArea.Right);
        renderArea.Bottom = (short)Math.Max(endY, renderArea.Bottom);
        needsRender = true;
    }

    public void RenderBuffer()
    {
        if (!needsRender) return;

        fixed (CHARINFO* arrayPtr = &charInfoBuffer[0])
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

        renderArea = _defaultRenderArea;
    }
}