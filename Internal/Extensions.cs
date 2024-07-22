namespace ConsoleNexusEngine.Internal;

using System.Collections;

internal static class Extensions
{
    public static bool FastHasFlag(this NexusInputType type, NexusInputType flag)
        => (type & flag) != NexusInputType.None;

    public static bool IsInRange(in this NexusCoord coord, in NexusCoord start, in NexusCoord end)
        => coord.X >= start.X && coord.Y >= start.Y && coord.X <= end.X && coord.Y <= end.Y;

    public static bool IsInRange(in this NexusCoord coord, in NexusCoord start, in NexusSize range)
        => coord.X >= start.X && coord.Y >= start.Y && coord.X <= start.X + range.Width && coord.Y <= start.Y + range.Height;

    public static int GetKey(this IReadOnlyDictionary<ConsoleColor, NexusColor> dictionary, in NexusColor color)
    {
        foreach (var pair in dictionary)
        {
            if (pair.Value == color) return (int)pair.Key;
        }

        return -1;
    }

    public static bool TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue? value, out TKey key) where TValue : notnull
    {
        if (value is not null)
        {
            foreach (var pair in dictionary)
            {
                if (pair.Value.Equals(value))
                {
                    key = pair.Key;
                    return true;
                }
            }
        }

        key = default!;
        return false;
    }

    public static bool IsInRange<T>(this Memory2D<T> memory, in NexusCoord coord) where T : struct
        => coord.X >= 0 && coord.X < memory.Width && coord.Y >= 0 && coord.Y < memory.Height;

    public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> source)
        => source as IReadOnlyCollection<T> ?? new ReadOnlyCollectionAdapter<T>(source);

    private sealed class ReadOnlyCollectionAdapter<T> : IReadOnlyCollection<T>
    {
        private readonly ICollection<T> _source;

        public ReadOnlyCollectionAdapter(ICollection<T> source) => _source = source;

        public int Count => _source.Count;

        public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}