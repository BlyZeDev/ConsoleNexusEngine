namespace ConsoleNexusEngine.Internal;

internal readonly ref struct ReadOnlySpan2D<T> where T : struct
{
    private readonly ReadOnlySpan<T> _span;

    public readonly int Width { get; }
    public readonly int Height { get; }

    public readonly int Length => _span.Length;

    public ReadOnlySpan2D(scoped in ReadOnlySpan<T> span, in int width, in int height)
    {
        _span = span;
        Width = width;
        Height = height;
    }

    public ReadOnlySpan2D(in int width, in int height) : this(new T[width * height], width, height) { }

    public ref readonly T this[in int i] => ref _span[i];

    public ref readonly T this[in int x, in int y] => ref _span[Width * y + x];

    public ReadOnlySpan2D<T> Resize(in int width, in int height)
    {
        var newSpan = new T[width * height].AsSpan();

        _span.TryCopyTo(newSpan);

        return new ReadOnlySpan2D<T>(newSpan, width, height);
    }
}