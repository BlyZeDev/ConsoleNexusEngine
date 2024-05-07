namespace ConsoleNexusEngine.IO;

using System.Collections.Immutable;

/// <summary>
/// Represents a collection of pressed keys and the current mouse position
/// </summary>
public readonly record struct NexusInputCollection
{
    internal static NexusInputCollection Empty => default;

    /// <summary>
    /// The mouse position
    /// </summary>
    public readonly NexusCoord MousePosition { get; }

    /// <summary>
    /// The pressed keys
    /// </summary>
    public readonly ImmutableArray<NexusKey> Keys { get; }

    /// <summary>
    /// Creates an empty <see cref="NexusInputCollection"/>
    /// </summary>
    public NexusInputCollection() : this(NexusCoord.MinValue, []) { }

    internal NexusInputCollection(in NexusCoord mousePosition) : this(mousePosition, []) { }

    internal NexusInputCollection(in NexusCoord mousePosition, in ImmutableArray<NexusKey> keys)
    {
        MousePosition = mousePosition;
        Keys = keys;
    }
}