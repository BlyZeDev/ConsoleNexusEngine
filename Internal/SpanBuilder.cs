namespace ConsoleNexusEngine.Internal;

using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

internal ref struct SpanBuilder<T>
{
    private T[] _arrayToReturnToPool;
    private Span<T> _values;
    private int _pos;

    public SpanBuilder() : this(Span<T>.Empty) { }

    public SpanBuilder(Span<T> initialBuffer)
    {
        _arrayToReturnToPool = Array.Empty<T>();
        _values = initialBuffer;
        _pos = 0;
    }

    public int Length
    {
        readonly get => _pos;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Length)} can not be smaller than 0");
            if (value > _values.Length) throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Length)} can not be greater than {nameof(Capacity)}");
            _pos = value;
        }
    }

    public readonly int Capacity => _values.Length;

    public ref T this[int index] => ref _values[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(T c)
    {
        int pos = _pos;
        if ((uint)pos < (uint)_values.Length)
        {
            _values[pos] = c;
            _pos = pos + 1;
        }
        else GrowAndAppend(c);
    }

    public void Append(ReadOnlySpan<T> value)
    {
        int pos = _pos;
        if (pos > _values.Length - value.Length) Grow(value.Length);

        value.CopyTo(_values[_pos..]);
        _pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(T c)
    {
        Grow(1);
        Append(c);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_pos > _values.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        T[] poolArray = ArrayPool<T>.Shared.Rent(Math.Max(_pos + additionalCapacityBeyondPos, _values.Length * 2));

        _values[.._pos].CopyTo(poolArray);

        T[] toReturn = _arrayToReturnToPool;
        _values = _arrayToReturnToPool = poolArray;
        ArrayPool<T>.Shared.Return(toReturn);
    }

    public readonly Span<T> AsSpan() => _values[.._pos];
    public readonly Span<T> AsSpan(int start) => _values[start.._pos];
    public readonly Span<T> AsSpan(int start, int length) => _values.Slice(start, length);

    public readonly ReadOnlySpan<T> AsReadOnlySpan() => _values[.._pos];
    public readonly ReadOnlySpan<T> AsReadOnlySpan(int start) => _values[start.._pos];
    public readonly ReadOnlySpan<T> AsReadOnlySpan(int start, int length) => _values.Slice(start, length);
}