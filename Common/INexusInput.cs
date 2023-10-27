namespace ConsoleNexusEngine.Common;

using System;

/// <summary>
/// Represents a generic interface for inputs
/// </summary>
public interface INexusInput
{
    /// <summary>
    /// The exact time the input was made
    /// </summary>
    public DateTime PressedAt { get; }

    /// <summary>
    /// The key that got pressed
    /// </summary>
    public NexusKey Key { get; }
}