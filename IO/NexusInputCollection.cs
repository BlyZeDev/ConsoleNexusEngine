namespace ConsoleNexusEngine.IO;

using System.Collections.Immutable;

/// <summary>
/// Represents a collection of pressed keys, the current mouse position and the gamepad state
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
    /// The state of all gamepads
    /// </summary>
    /// <remarks>
    /// This array ALWAYS contains 4 gamepads ordered by player number.<br/>Index 0 = player 1.
    /// </remarks>
    public readonly ImmutableArray<NexusGamepad> Gamepads { get; }

    /// <summary>
    /// Creates an empty <see cref="NexusInputCollection"/>
    /// </summary>
    public NexusInputCollection() : this(NexusCoord.MinValue, [], [NexusGamepad.Empty, NexusGamepad.Empty, NexusGamepad.Empty, NexusGamepad.Empty]) { }

    internal NexusInputCollection(in NexusCoord mousePosition) : this(mousePosition, [], [NexusGamepad.Empty, NexusGamepad.Empty, NexusGamepad.Empty, NexusGamepad.Empty]) { }

    internal NexusInputCollection(in NexusCoord mousePosition, in ImmutableArray<NexusKey> keys, in ImmutableArray<NexusGamepad> gamepads)
    {
        MousePosition = mousePosition;
        Keys = keys;
        Gamepads = gamepads;
    }
}