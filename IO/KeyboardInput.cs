namespace ConsoleNexusEngine.IO;

using ConsoleNexusEngine;

/// <summary>
/// Represents a keyboard input
/// </summary>
public readonly record struct KeyboardInput : INexusInput
{
    /// <inheritdoc/>
    public NexusKey Key { get; }

    /// <inheritdoc/>
    public Coord? MousePosition => null;

    /// <summary>
    /// Initializes a new Keyboard Input
    /// </summary>
    /// <param name="key"></param>
    public KeyboardInput(NexusKey key) => Key = key;
}