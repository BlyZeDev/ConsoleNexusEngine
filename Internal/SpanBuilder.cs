namespace ConsoleNexusEngine.Internal;

using System.Buffers;
using System.Runtime.CompilerServices;

internal ref struct SpanBuilder<T>
{
    private T[] _arrayToReturnToPool;
    private Span<T> _values;
    private int _pos;

    public SpanBuilder() : this([]) { }

    public SpanBuilder(Span<T> initialBuffer)
    {
        _arrayToReturnToPool = [];
        _values = initialBuffer;
        _pos = 0;
    }

    public int Length
    {
        readonly get => _pos;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, _values.Length);
            _pos = value;
        }
    }

    public readonly int Capacity => _values.Length;

    public ref T this[int index] => ref _values[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(T value)
    {
        if (_pos < _values.Length)
        {
            _values[_pos] = value;
            _pos++;
            return;
        }
        
        GrowAndAppend(value);
    }

    public void Append(ReadOnlySpan<T> value)
    {
        if (_pos > _values.Length - value.Length) Grow(value.Length);

        value.CopyTo(_values[_pos..]);
        _pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(T value)
    {
        Grow(1);
        Append(value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacity)
    {
        if (additionalCapacity < 1) return;
        if (_pos <= _values.Length - additionalCapacity) return;

        T[] poolArray = ArrayPool<T>.Shared.Rent(Math.Max(_pos + additionalCapacity, _values.Length * 2));

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