namespace ConsoleNexusEngine.IO;

using System.Collections.Immutable;

/// <summary>
/// Represents a collection of pressed keys and the current mouse position
/// </summary>
public readonly record struct NexusInputCollection
{
    /// <summary>
    /// The mouse position
    /// </summary>
    public readonly Coord MousePosition { get; }

    /// <summary>
    /// The pressed keys
    /// </summary>
    public readonly ImmutableArray<NexusKey> Keys { get; }

    /// <summary>
    /// Creates an empty <see cref="NexusInputCollection"/>
    /// </summary>
    public NexusInputCollection() : this(Coord.MinValue, []) { }

    internal NexusInputCollection(Coord mousePosition) : this(mousePosition, []) { }

    internal NexusInputCollection(Coord mousePosition, ImmutableArray<NexusKey> keys)
    {
        MousePosition = mousePosition;
        Keys = keys;
    }
}