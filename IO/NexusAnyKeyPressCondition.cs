namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents an any key press condition
/// </summary>
public readonly struct NexusAnyKeyPressCondition : INexusInputCondition
{
    private readonly ReadOnlyMemory<NexusKey> _keys;

    /// <summary>
    /// Initializes an empty key press condition
    /// </summary>
    public NexusAnyKeyPressCondition() : this([]) { }

    /// <summary>
    /// Checks if any key in a collection is pressed
    /// </summary>
    /// <param name="keys">The keys that where at least one have to be pressed</param>
    public NexusAnyKeyPressCondition(in ReadOnlySpan<NexusKey> keys)
        : this(keys.ToArray()) { }

    /// <summary>
    /// Checks if any key in a collection is pressed
    /// </summary>
    /// <param name="keys">The keys that where at least one have to be pressed</param>
    public NexusAnyKeyPressCondition(params NexusKey[] keys) => _keys = keys;

    /// <inheritdoc/>
    public bool Check(in NexusKey key)
    {
        var keySpan = _keys.Span;
        foreach (var item in keySpan)
        {
            if (item == key) return true;
        }

        return false;
    }

    /// <summary>
    /// Returns <see langword="false"/>
    /// </summary>
    /// <param name="mousePosition"><inheritdoc/></param>
    /// <returns><see cref="bool"/></returns>
    public bool Check(in NexusCoord mousePosition) => false;
}