namespace ConsoleNexusEngine.Helpers;

/// <summary>
/// Useful methods for <see cref="NexusCoord"/> navigation
/// </summary>
public static class NexusNavigation
{
    /// <summary>
    /// Moves the <paramref name="coordinate"/> in specific directions
    /// </summary>
    /// <param name="coordinate">The coordinate to move</param>
    /// <param name="direction">The direction in which to move</param>
    /// <param name="steps">The amount of steps to go in each direction</param>
    public static void Move(ref NexusCoord coordinate, in NexusDirection direction, in int steps)
    {
        var (x, y) = coordinate;

        if (direction.HasDirection(NexusDirection.Left)) x -= steps;
        if (direction.HasDirection(NexusDirection.Right)) x += steps;
        if (direction.HasDirection(NexusDirection.Up)) y -= steps;
        if (direction.HasDirection(NexusDirection.Down)) y += steps;

        coordinate = new NexusCoord(x, y);
    }

    /// <summary>
    /// Moves the <paramref name="coordinate"/> in specific directions
    /// </summary>
    /// <param name="coordinate">The coordinate to move</param>
    /// <param name="direction">The direction in which to move</param>
    /// <returns><see cref="NexusCoord"/></returns>
    public static void Move(ref NexusCoord coordinate, in NexusDirection direction)
        => Move(ref coordinate, direction, 1);

    /// <summary>
    /// Moves the <paramref name="coordinate"/> to the left
    /// </summary>
    /// <param name="coordinate">The coordinate to move</param>
    /// <param name="steps">The amount of steps to go left</param>
    /// <returns><see cref="NexusCoord"/></returns>
    public static void MoveLeft(ref NexusCoord coordinate, in int steps) => Move(ref coordinate, NexusDirection.Left, steps);

    /// <summary>
    /// Moves the <paramref name="coordinate"/> to the left
    /// </summary>
    /// <param name="coordinate">The coordinate to move</param>
    /// <returns><see cref="NexusCoord"/></returns>
    public static void MoveLeft(ref NexusCoord coordinate) => MoveLeft(ref coordinate, 1);

    /// <summary>
    /// Moves the <paramref name="coordinate"/> to the right
    /// </summary>
    /// <param name="coordinate">The coordinate to move</param>
    /// <param name="steps">The amount of steps to go left</param>
    /// <returns><see cref="NexusCoord"/></returns>
    public static void MoveRight(ref NexusCoord coordinate, in int steps) => Move(ref coordinate, NexusDirection.Right, steps);

    /// <summary>
    /// Moves the <paramref name="coordinate"/> to the right
    /// </summary>
    /// <param name="coordinate">The coordinate to move</param>
    /// <returns><see cref="NexusCoord"/></returns>
    public static void MoveRight(ref NexusCoord coordinate) => MoveRight(ref coordinate, 1);

    /// <summary>
    /// Moves the <paramref name="coordinate"/> up
    /// </summary>
    /// <param name="coordinate">The coordinate to move</param>
    /// <param name="steps">The amount of steps to go left</param>
    /// <returns><see cref="NexusCoord"/></returns>
    public static void MoveUp(ref NexusCoord coordinate, in int steps) => Move(ref coordinate, NexusDirection.Up, steps);

    /// <summary>
    /// Moves the <paramref name="coordinate"/> up
    /// </summary>
    /// <param name="coordinate">The coordinate to move</param>
    /// <returns><see cref="NexusCoord"/></returns>
    public static void MoveUp(ref NexusCoord coordinate) => MoveUp(ref coordinate, 1);

    /// <summary>
    /// Moves the <paramref name="coordinate"/> down
    /// </summary>
    /// <param name="coordinate">The coordinate to move</param>
    /// <param name="steps">The amount of steps to go left</param>
    /// <returns><see cref="NexusCoord"/></returns>
    public static void MoveDown(ref NexusCoord coordinate, in int steps) => Move(ref coordinate, NexusDirection.Down, steps);

    /// <summary>
    /// Moves the <paramref name="coordinate"/> down
    /// </summary>
    /// <param name="coordinate">The coordinate to move</param>
    /// <returns><see cref="NexusCoord"/></returns>
    public static void MoveDown(ref NexusCoord coordinate) => MoveDown(ref coordinate, 1);
}