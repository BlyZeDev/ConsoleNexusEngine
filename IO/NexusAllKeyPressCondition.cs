namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents an all key press condition
/// </summary>
public readonly struct NexusAllKeyPressCondition : INexusInputCondition
{
    private readonly ReadOnlyMemory<NexusKey> _keys;

    /// <summary>
    /// Initializes an empty key press condition
    /// </summary>
    public NexusAllKeyPressCondition() : this([]) { }

    /// <summary>
    /// Checks if all keys in a collection are pressed
    /// </summary>
    /// <param name="keys">The keys that ALL have to be pressed</param>
    public NexusAllKeyPressCondition(in ReadOnlySpan<NexusKey> keys)
        : this(keys.ToArray()) { }

    /// <summary>
    /// Checks if all keys in a collection are pressed
    /// </summary>
    /// <param name="keys">The keys that ALL have to be pressed</param>
    public NexusAllKeyPressCondition(params NexusKey[] keys) => _keys = keys;

    /// <inheritdoc/>
    public bool Check(in NexusKey key)
    {
        var keySpan = _keys.Span;
        foreach (var item in keySpan)
        {
            if (item != key) return false;
        }

        return true;
    }

    /// <summary>
    /// Returns <see langword="false"/>
    /// </summary>
    /// <param name="mousePosition"><inheritdoc/></param>
    /// <returns><see cref="bool"/></returns>
    public bool Check(in NexusCoord mousePosition) => false;
}