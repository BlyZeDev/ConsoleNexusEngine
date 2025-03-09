namespace ConsoleNexusEngine.IO;

using System.Collections;

/// <summary>
/// A unique collection of <see cref="NexusKey"/>
/// </summary>
/// <remarks>
/// Basically a Readonly <see cref="HashSet{T}"/> of <see cref="NexusKey"/>
/// </remarks>
public sealed class NexusKeyCollection : IEnumerable<NexusKey>
{
    private readonly HashSet<NexusKey> _keys;

    /// <summary>
    /// The amount of keys
    /// </summary>
    public int Count => _keys.Count;

    internal NexusKeyCollection() => _keys = [];

    /// <summary>
    /// <see langword="true"/> if the key is present in the collection, otherwise <see langword="false"/>
    /// </summary>
    /// <param name="key">The key to check for</param>
    /// <returns><see cref="bool"/></returns>
    public bool Contains(in NexusKey key) => _keys.Contains(key);

    /// <inheritdoc/>
    public IEnumerator<NexusKey> GetEnumerator() => _keys.GetEnumerator();

    internal void Add(in NexusKey key) => _keys.Add(key);

    internal void RemoveWhere(Predicate<NexusKey> match) => _keys.RemoveWhere(match);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}