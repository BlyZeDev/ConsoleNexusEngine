namespace ConsoleNexusEngine.Internal;

internal readonly struct Memory2D<T> where T : struct
{
    private readonly Memory<T> _memory;

    public readonly Span<T> Span => _memory.Span;

    public readonly int Width { get; }
    public readonly int Height { get; }

    public readonly int Length => _memory.Length;

    public Memory2D(in Memory<T> memory, in int width, in int height)
    {
        _memory = memory;
        Width = width;
        Height = height;
    }

    public Memory2D(in int width, in int height) : this(new T[width * height], width, height) { }

    public readonly ref T this[in int i] => ref _memory.Span[i];

    public readonly ref T this[in int x, in int y] => ref _memory.Span[IndexDimensions.Get1D(x, y, Width)];

    public readonly ReadOnlyMemory2D<T> ToReadOnly() => new(_memory, Width, Height);
}