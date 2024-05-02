namespace ConsoleNexusEngine.Internal;

using System.Buffers;
using System.Runtime.CompilerServices;

internal ref struct SpanBuilder<T>
{
    private T[] _arrayToReturnToPool;
    private Span<T> values;
    private int pos;

    public SpanBuilder() : this([]) { }

    public SpanBuilder(scoped in Span<T> initialBuffer)
    {
        _arrayToReturnToPool = [];
        values = initialBuffer;
        pos = 0;
    }

    public int Length
    {
        readonly get => pos;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, values.Length);
            pos = value;
        }
    }

    public readonly int Capacity => values.Length;

    public ref readonly T this[in int index] => ref values[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(T value)
    {
        if (pos < values.Length)
        {
            values[pos] = value;
            pos++;
            return;
        }
        
        GrowAndAppend(value);
    }

    public void Append(in ReadOnlySpan<T> value)
    {
        if (pos > values.Length - value.Length) Grow(value.Length);

        value.CopyTo(values[pos..]);
        pos += value.Length;
    }

    public readonly bool Contains(T value) => values.BinarySearch(value, Comparer<T>.Default) > -1;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(T value)
    {
        Grow(1);
        Append(value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(in int additionalCapacity)
    {
        if (additionalCapacity < 1) return;
        if (pos <= values.Length - additionalCapacity) return;

        T[] poolArray = ArrayPool<T>.Shared.Rent(Math.Max(pos + additionalCapacity, values.Length * 2));

        values[..pos].CopyTo(poolArray);
        
        T[] toReturn = _arrayToReturnToPool;
        values = _arrayToReturnToPool = poolArray;
        ArrayPool<T>.Shared.Return(toReturn);
    }

    public readonly Span<T> AsSpan() => values[..pos];
    public readonly Span<T> AsSpan(in int start) => values[start..pos];
    public readonly Span<T> AsSpan(in int start, in int length) => values.Slice(start, length);

    public readonly ReadOnlySpan<T> AsReadOnlySpan() => values[..pos];
    public readonly ReadOnlySpan<T> AsReadOnlySpan(in int start) => values[start..pos];
    public readonly ReadOnlySpan<T> AsReadOnlySpan(in int start, in int length) => values.Slice(start, length);
}