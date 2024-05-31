namespace ConsoleNexusEngine.Internal;

internal readonly struct ReadOnlyMemory2D<T> where T : struct
{
    private readonly ReadOnlyMemory<T> _memory;

    public readonly ReadOnlySpan<T> Span => _memory.Span;

    public readonly int Width { get; }
    public readonly int Height { get; }

    public readonly int Length => _memory.Length;

    public ReadOnlyMemory2D(in ReadOnlyMemory<T> memory, in int width, in int height)
    {
        _memory = memory;
        Width = width;
        Height = height;
    }

    public ReadOnlyMemory2D(in int width, in int height) : this(new T[width * height], width, height) { }

    public readonly ref readonly T this[in int i] => ref _memory.Span[i];

    public readonly ref readonly T this[in int x, in int y] => ref _memory.Span[Width * y + x];

    public readonly ReadOnlyMemory2D<T> Resize(in int width, in int height)
    {
        var newMemory = new T[width * height].AsMemory();

        _memory.TryCopyTo(newMemory);

        return new ReadOnlyMemory2D<T>(newMemory, width, height);
    }
}