namespace ConsoleNexusEngine.Internal;

internal readonly struct Memory2D<T> where T : struct
{
    private readonly Memory<T> _memory;

    public readonly Span<T> Span => _memory.Span;

    public readonly int Width { get; }
    public readonly int Height { get; }

    public readonly int Length => _memory.Length;

    public Memory2D(T[] array, in int width, in int height)
    {
        _memory = array;
        Width = width;
        Height = height;
    }

    public Memory2D(in int width, in int height) : this(new T[width * height], width, height) { }

    public readonly T this[in int i]
    {
        get => _memory.Span[i];
        set => _memory.Span[i] = value;
    }

    public readonly T this[in int x, in int y]
    {
        get => _memory.Span[Width * y + x];
        set => _memory.Span[Width * y + x] = value;
    }

    public Memory2D<T> Resize(in int width, in int height)
    {
        var newArr = new T[width * height];

        _memory.TryCopyTo(newArr);

        return new Memory2D<T>(newArr, width, height);
    }

    public ReadOnlyMemory2D<T> ToReadOnly() => new(_memory, Width, Height);
}