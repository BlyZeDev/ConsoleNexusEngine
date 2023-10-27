namespace ConsoleNexusEngine.Common;

using System;

/// <summary>
/// Represents a key pressed on the mouse
/// </summary>
public readonly record struct MouseInput : INexusInput
{
    /// <inheritdoc/>
    public DateTime PressedAt { get; }

    /// <summary>
    /// <inheritdoc/>, 0 if none got pressed
    /// </summary>
    public NexusKey Key { get; }

    /// <summary>
    /// The coordinates where the mouse button was pressed
    /// </summary>
    public Coord Coordinates { get; }

    internal MouseInput(DateTime pressedAt, NexusKey key, Coord coordinates)
    {
        PressedAt = pressedAt;
        Key = key;
        Coordinates = coordinates;
    }
}