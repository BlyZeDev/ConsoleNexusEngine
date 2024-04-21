namespace ConsoleNexusEngine.Internal;

internal readonly struct ReadOnlyMemory2D<T> where T : struct
{
    private readonly ReadOnlyMemory<T> _memory;

    public readonly ReadOnlySpan<T> Span => _memory.Span;

    public readonly int Width { get; }
    public readonly int Height { get; }

    public readonly int Length => _memory.Length;

    public ReadOnlyMemory2D(Memory<T> memory, in int width, in int height)
    {
        _memory = memory;
        Width = width;
        Height = height;
    }

    public ReadOnlyMemory2D(in int width, in int height) : this(new T[width * height], width, height) { }

    public readonly T this[in int i] => _memory.Span[i];

    public readonly T this[in int x, in int y] => _memory.Span[Width * y + x];

    public ReadOnlyMemory2D<T> Resize(in int width, in int height)
    {
        var newArr = new T[width * height];

        _memory.TryCopyTo(newArr);

        return new ReadOnlyMemory2D<T>(newArr, width, height);
    }
}