namespace ConsoleNexusEngine.IO;

using ConsoleNexusEngine;

/// <summary>
/// Represents a mouse input
/// </summary>
public readonly record struct MouseInput : INexusInput
{
    /// <inheritdoc/>
    public NexusKey Key { get; }

    /// <inheritdoc/>
    public Coord? MousePosition { get; }

    /// <summary>
    /// Initializes a new Mouse Position
    /// </summary>
    /// <param name="key">The key that got pressed</param>
    /// <param name="mousePosition">The position of the mouse cursor</param>
    public MouseInput(NexusKey key, Coord mousePosition)
    {
        Key = key;
        MousePosition = mousePosition;
    }

    /// <summary>
    /// Initializes a new Mouse Position
    /// </summary>
    /// <param name="mousePosition">The position of the mouse cursor</param>
    public MouseInput(Coord mousePosition) : this(NexusKey.None, mousePosition) { }
}