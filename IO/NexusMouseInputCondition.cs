namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents a mouse input condition
/// </summary>
public readonly struct NexusMouseInputCondition : INexusInputCondition
{
    private readonly NexusCoord _mousePos;

    /// <summary>
    /// Initializes an empty mouse position check
    /// </summary>
    public NexusMouseInputCondition() : this(NexusCoord.MaxValue) { }

    /// <summary>
    /// Checks for a mouse position
    /// </summary>
    /// <param name="mousePosition">The position to check for</param>
    public NexusMouseInputCondition(in NexusCoord mousePosition) => _mousePos = mousePosition;

    /// <inheritdoc/>
    public bool Check<T>(T toCheck) where T : struct
        => toCheck is NexusCoord coord && _mousePos == coord;
}