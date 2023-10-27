namespace ConsoleNexusEngine.Common;

using System;

/// <summary>
/// Represents a key pressed on the keyboard
/// </summary>
public readonly record struct KeyboardInput : INexusInput
{
    /// <inheritdoc/>
    public DateTime PressedAt { get; }

    /// <inheritdoc/>
    public NexusKey Key { get; }

    /// <summary>
    /// Initializes a new key pressed event
    /// </summary>
    /// <param name="pressedAt">The exact time the key was pressed</param>
    /// <param name="key">The key that was pressed</param>
    internal KeyboardInput(DateTime pressedAt, NexusKey key)
    {
        PressedAt = pressedAt;
        Key = key;
    }
}