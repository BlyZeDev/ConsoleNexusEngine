﻿namespace ConsoleNexusEngine.Internal;

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

    public readonly ref T this[in int i] => ref _span[i];

    public readonly ref T this[in int x, in int y] => ref _span[MathHelper.GetIndex(x, y, Width)];

    public readonly Span2D<T> Resize(in int width, in int height)
    {
        var newSpan = new T[width * height].AsSpan();

        _span.TryCopyTo(newSpan);

        return new Span2D<T>(newSpan, width, height);
    }

    public readonly Span<T> As1D() => _span;

    public readonly ReadOnlySpan2D<T> ToReadOnly()
    {
        var readOnly = new ReadOnlySpan2D<T>(_span, Width, Height);
        return readOnly;
    }
}