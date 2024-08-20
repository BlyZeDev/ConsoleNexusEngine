namespace ConsoleNexusEngine.Helpers;

/// <summary>
/// Represents a direction
/// </summary>
/// <remarks>
/// This enum has flags.
/// </remarks>
[Flags]
public enum NexusDirection
{
    /// <summary>
    /// No direction
    /// </summary>
    None = 0,
    /// <summary>
    /// The west direction
    /// </summary>
    Left = 1,
    /// <summary>
    /// The east direction
    /// </summary>
    Right = 2,
    /// <summary>
    /// The north direction
    /// </summary>
    Up = 4,
    /// <summary>
    /// The south direction
    /// </summary>
    Down = 8
}