namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents a mouse range input condition
/// </summary>
public readonly struct NexusMouseRangeInputCondition : INexusInputCondition
{
    private readonly NexusCoord _startPos;
    private readonly NexusCoord _endPos;

    /// <summary>
    /// Initializes an empty mouse position check
    /// </summary>
    public NexusMouseRangeInputCondition() : this(NexusCoord.MaxValue, NexusCoord.MaxValue) { }

    /// <summary>
    /// Checks if a mouse position is in range
    /// </summary>
    /// <param name="start">The start coordinate, top-left</param>
    /// <param name="end">The end coordinate, bottom-right</param>
    public NexusMouseRangeInputCondition(in NexusCoord start, in NexusCoord end)
    {
        _startPos = start;
        _endPos = end;
    }

    /// <summary>
    /// Checks if a mouse position is in range
    /// </summary>
    /// <param name="start">The start coordinate, top-left</param>
    /// <param name="range">The range beginning from the start coordinate</param>
    public NexusMouseRangeInputCondition(in NexusCoord start, in NexusSize range)
        : this(start, start + range.ToCoord()) { }

    /// <inheritdoc/>
    public bool Check(in NexusKey key) => false;

    /// <inheritdoc/>
    public bool Check(in NexusCoord mousePosition)
        => mousePosition.IsInRange(_startPos, _endPos);
}