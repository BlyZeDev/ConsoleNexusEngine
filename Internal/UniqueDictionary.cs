namespace ConsoleNexusEngine.Internal;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

internal sealed class UniqueDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary;
    private readonly HashSet<TValue> _values;

    public UniqueDictionary(IEqualityComparer<TValue>? valueComparer = null)
    {
        _dictionary = [];
        _values = new HashSet<TValue>(valueComparer);
    }

    public TValue this[TKey key]
    {
        get => _dictionary[key];
        set => _dictionary[key] = value;
    }

    public ICollection<TKey> Keys => _dictionary.Keys;

    public ICollection<TValue> Values => _dictionary.Values;

    public int Count => _dictionary.Count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        if (!_values.Add(value)) throw new ArgumentException("The value is not unique", nameof(value));

        _dictionary.Add(key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    public void Clear()
    {
        _dictionary.Clear();
        _values.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);

    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        => ((ICollection)_dictionary).CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

    public bool Remove(TKey key)
    {
        var removed = _dictionary.Remove(key, out var removedItem);

        _values.Remove(removedItem!);

        return removed;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => _dictionary.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}