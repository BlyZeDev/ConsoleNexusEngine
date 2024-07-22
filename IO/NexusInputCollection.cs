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
    /// The state of the player 1 gamepad
    /// </summary>
    public readonly NexusGamepad Player1Gamepad { get; }

    /// <summary>
    /// The state of the player 2 gamepad
    /// </summary>
    public readonly NexusGamepad Player2Gamepad { get; }

    /// <summary>
    /// The state of the player 3 gamepad
    /// </summary>
    public readonly NexusGamepad Player3Gamepad { get; }

    /// <summary>
    /// The state of the player 4 gamepad
    /// </summary>
    public readonly NexusGamepad Player4Gamepad { get; }

    /// <summary>
    /// Creates an empty <see cref="NexusInputCollection"/>
    /// </summary>
    public NexusInputCollection() : this(NexusCoord.MinValue, [], NexusGamepad.Empty, NexusGamepad.Empty, NexusGamepad.Empty, NexusGamepad.Empty) { }

    internal NexusInputCollection(in NexusCoord mousePosition) : this(mousePosition, [], NexusGamepad.Empty, NexusGamepad.Empty, NexusGamepad.Empty, NexusGamepad.Empty) { }

    internal NexusInputCollection(in NexusCoord mousePosition, in ImmutableArray<NexusKey> keys, in NexusGamepad gamepad1, in NexusGamepad gamepad2, in NexusGamepad gamepad3, in NexusGamepad gamepad4)
    {
        MousePosition = mousePosition;
        Keys = keys;
        Player1Gamepad = gamepad1;
        Player2Gamepad = gamepad2;
        Player3Gamepad = gamepad3;
        Player4Gamepad = gamepad4;
    }
}