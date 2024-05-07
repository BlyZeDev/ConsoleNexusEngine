namespace ConsoleNexusEngine.Internal;

internal readonly ref struct Span2D<T> where T : struct
{
    private readonly Span<T> _span;

    public readonly int Width { get; }
    public readonly int Height { get; }

    public readonly int Length => _span.Length;

    public Span2D(scoped in Span<T> span, in int width, in int height)
    {
        _span = span;
        Width = width;
        Height = height;
    }

    public Span2D(in int width, in int height) : this(new T[width * height], width, height) { }

    public T this[in int i]
    {
        readonly get => _span[i];
        set => _span[i] = value;
    }

    public T this[in int x, in int y]
    {
        readonly get => _span[Width * y + x];
        set => _span[Width * y + x] = value;
    }

    public Span2D<T> Resize(in int width, in int height)
    {
        var newSpan = new T[width * height].AsSpan();

        _span.TryCopyTo(newSpan);

        return new Span2D<T>(newSpan, width, height);
    }

    public ReadOnlySpan2D<T> ToReadOnly()
    {
        var readOnly = new ReadOnlySpan2D<T>(_span, Width, Height);
        return readOnly;
    }
}