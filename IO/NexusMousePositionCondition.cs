namespace ConsoleNexusEngine.IO;

using System.Linq;

/// <summary>
/// Represents a mouse position input condition
/// </summary>
public sealed class NexusMousePositionCondition : INexusInputCondition
{
    private readonly Predicate<INexusInput> _predicate;

    private NexusMousePositionCondition(Predicate<INexusInput> predicate) => _predicate = predicate;

    /// <summary>
    /// Checks for a mouse position
    /// </summary>
    /// <param name="coord">The coordinate of the position to check for</param>
    public NexusMousePositionCondition(Coord coord)
        : this((input) => input.MousePosition == coord) { }

    /// <summary>
    /// Checks if the mouse position is any of <paramref name="coords"/>
    /// </summary>
    /// <param name="coords">The coordinates of the positions to check for</param>
    public NexusMousePositionCondition(params Coord[] coords)
        : this((input) => input.MousePosition.HasValue && coords.Contains(input.MousePosition.Value)) { }

    /// <summary>
    /// Checks if the mouse position is in range of <paramref name="fromCoord"/> and <paramref name="toCoord"/>
    /// </summary>
    /// <param name="fromCoord">The coordinate where the range starts</param>
    /// <param name="toCoord">The coordinate where the range ends</param>
    public NexusMousePositionCondition(Coord fromCoord, Coord toCoord)
        : this((input) => input.MousePosition.HasValue && input.MousePosition.Value.IsInRange(fromCoord, toCoord)) { }

    ///<inheritdoc/>
    public bool Check(INexusInput input) => _predicate(input);
}