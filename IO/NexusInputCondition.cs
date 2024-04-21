namespace ConsoleNexusEngine.IO;

using System.Linq;

/// <summary>
/// Represents an input condition
/// </summary>
public sealed class NexusInputCondition
{
    private readonly Predicate<NexusKey> _keyPressCondition;
    private readonly Predicate<Coord> _mousePosCondition;

    internal readonly bool _isMousePosCondition;

    private NexusInputCondition(Predicate<NexusKey> keyPressCondition, Predicate<Coord> mousePosCondition, in bool isMousePosCondition)
    {
        _keyPressCondition = keyPressCondition;
        _mousePosCondition = mousePosCondition;

        _isMousePosCondition = isMousePosCondition;
    }

    private NexusInputCondition(Predicate<NexusKey> keyPressCondition)
        : this(keyPressCondition, (coord) => false, false) { }

    private NexusInputCondition(Predicate<Coord> mousePosCondition)
        : this((key) => false, mousePosCondition, true) { }

    /// <summary>
    /// Checks for a key press
    /// </summary>
    /// <param name="key">The key to check for</param>
    public NexusInputCondition(NexusKey key) : this((toCheck) => toCheck == key) { }

    /// <summary>
    /// Checks for a mouse position
    /// </summary>
    /// <param name="mousePosition">The mouse position to check for</param>
    public NexusInputCondition(Coord mousePosition) : this((toCheck) => toCheck == mousePosition) { }

    /// <summary>
    /// Checks if any key is pressed
    /// </summary>
    /// <param name="keys">The keys to check for</param>
    /// <returns><see cref="NexusInputCondition"/></returns>
    public static NexusInputCondition Any(params NexusKey[] keys) => new((toCheck) => keys.Contains(toCheck));

    /// <summary>
    /// Checks if all keys are pressed
    /// </summary>
    /// <param name="keys">The keys to check for</param>
    /// <returns><see cref="NexusInputCondition"/></returns>
    public static NexusInputCondition All(params NexusKey[] keys) => new((toCheck) => keys.All(x => x == toCheck));

    internal bool Check(NexusKey key) => _keyPressCondition(key);
    internal bool Check(Coord mousePos) => _mousePosCondition(mousePos);
}