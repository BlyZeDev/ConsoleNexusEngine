namespace ConsoleNexusEngine.Internal;

using System.Runtime.CompilerServices;

internal sealed unsafe class ConsoleBuffer
{
    private readonly nint _standardOutput;

    private CHAR_INFO[] charInfoBuffer;

    private bool needsRender;
    private SMALL_RECT renderArea;

    public short Width { get; private set; }
    public short Height { get; private set; }

    public ConsoleBuffer(in nint standardOutput, in int width, in int height)
    {
        _standardOutput = standardOutput;

        Width = (short)width;
        Height = (short)height;

        charInfoBuffer = new CHAR_INFO[Width * Height];
        needsRender = false;
        renderArea = default;
    }

    public void ChangeDimensions(in int width, in int height)
    {
        Width = (short)width;
        Height = (short)height;

        Array.Resize(ref charInfoBuffer, Width * Height);

        SetRedraw(0, 0, Width, Height);
    }

    public void ClearBuffer()
    {
        fixed (CHAR_INFO* ptr = &charInfoBuffer[0])
        {
            Unsafe.InitBlockUnaligned(ptr, 0, (uint)(charInfoBuffer.Length * sizeof(CHAR_INFO)));
        }

        needsRender = true;
    }

    public CHAR_INFO GetChar(in int x, in int y) => charInfoBuffer[MathHelper.GetIndex(x, y, Width)];

    public void SetChar(in int x, in int y, CHAR_INFO character)
    {
        var index = MathHelper.GetIndex(x, y, Width);

        ref var before = ref charInfoBuffer[index];

        if (Unsafe.ReadUnaligned<long>(Unsafe.AsPointer(ref before)) == Unsafe.ReadUnaligned<long>(Unsafe.AsPointer(ref character)))
            return;

        before = character;
        SetRedraw(x, y, x, y);
    }

    public void BlockSetChar(in int index, in ReadOnlySpan<CHAR_INFO> characterBlock, in int length)
        => characterBlock.CopyTo(charInfoBuffer.AsSpan().Slice(index, length));

    public void RenderBuffer()
    {
        if (!needsRender) return;

        needsRender = false;

        fixed (CHAR_INFO* arrayP = &charInfoBuffer[0])
        {
            fixed (SMALL_RECT* redrawAreaPtr = &renderArea)
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
                redrawAreaPtr);
            }
        }
    }

    private void SetRedraw(in int startX, in int startY, in int endX, in int endY)
    {
        renderArea.Left = (short)Math.Min(startX, renderArea.Left);
        renderArea.Top = (short)Math.Min(startY, renderArea.Top);
        renderArea.Right = (short)Math.Max(endX, renderArea.Right);
        renderArea.Bottom = (short)Math.Max(endY, renderArea.Bottom);
        needsRender = true;
    }
}