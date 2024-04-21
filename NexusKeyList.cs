namespace ConsoleNexusEngine;

using System.Buffers;
using System.Runtime.CompilerServices;

/// <summary>
/// Represents a list of <see cref="NexusKey"/>s
/// </summary>
public struct NexusKeyList
{
    private const int DefaultCapacity = 4;

    private readonly MemoryPool<NexusKey> _pool = MemoryPool<NexusKey>.Shared;

    private IMemoryOwner<NexusKey> _owner;
    private int count;
    private int capacity;

    /// <summary>
    /// Initializes a new <see cref="NexusKeyList"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NexusKeyList()
    {
        _owner = _pool.Rent(DefaultCapacity);
        capacity = DefaultCapacity;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Add(NexusKey key)
    {
        if (count == capacity)
        {
            var newCapacity = capacity * 2;
            var newOwner = _pool.Rent(newCapacity);

            _owner.Memory.CopyTo(newOwner.Memory);

            _owner = newOwner;
            capacity = newCapacity;
        }

        _owner.Memory.Span[count++] = key;
    }

    /// <summary>
    /// Returns the <see cref="NexusKey"/> at this index
    /// </summary>
    /// <param name="index">The index</param>
    /// <returns><see cref="NexusKey"/></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public readonly NexusKey this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => index < 0 || index >= count ? throw new IndexOutOfRangeException() : _owner.Memory.Span[index];
    }

    /// <summary>
    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    /// </summary>
    /// <returns><see cref="Enumerator"/></returns>
    public readonly Enumerator GetEnumerator() => new(_owner.Memory);

    /// <summary>
    /// <inheritdoc cref="IEnumerator{T}"/>
    /// </summary>
    public struct Enumerator
    {
        private readonly Memory<NexusKey> _memory;

        private int index;

        /// <summary>
        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        /// </summary>
        public readonly NexusKey Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _memory.Span[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(Memory<NexusKey> memory)
        {
            _memory = memory;
            index = -1;
        }

        /// <summary>
        /// <see langword="true"/> if there is a next entry, otherwise <see langword="false"/>
        /// </summary>
        /// <returns><see cref="bool"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var tempIndex = index + 1;

            if (index < _memory.Length)
            {
                index = tempIndex;
                return true;
            }

            return false;
        }
    }
}