namespace ConsoleNexusEngine;

using System.Numerics;

/// <summary>
/// Represents a positive coordinate
/// </summary>
public readonly record struct NexusCoord
    : IAdditionOperators<NexusCoord, NexusCoord, NexusCoord>,
    ISubtractionOperators<NexusCoord, NexusCoord, NexusCoord>,
    IMultiplyOperators<NexusCoord, NexusCoord, NexusCoord>,
    IDivisionOperators<NexusCoord, NexusCoord, NexusCoord>,
    IMinMaxValue<NexusCoord>,
    IDecrementOperators<NexusCoord>,
    IIncrementOperators<NexusCoord>
{
    /// <inheritdoc/>
    public static NexusCoord MinValue => new(0, 0);

    /// <inheritdoc/>
    public static NexusCoord MaxValue => new(int.MaxValue, int.MaxValue);

    /// <summary>
    /// The X coordinate
    /// </summary>
    public readonly int X { get; }

    /// <summary>
    /// The Y coordinate
    /// </summary>
    public readonly int Y { get; }

    /// <summary>
    /// Initializes a new <see cref="NexusCoord"/>
    /// </summary>
    public NexusCoord() : this(0, 0) { }

    /// <summary>
    /// Initializes a new <see cref="NexusCoord"/>
    /// </summary>
    /// <remarks>
    /// The coordinates are clamped between <see cref="MinValue"/> and <see cref="MaxValue"/>
    /// </remarks>
    /// <param name="x">The X coordinate</param>
    /// <param name="y">The Y coordinate</param>
    public NexusCoord(in int x, in int y)
    {
        X = Math.Clamp(x, 0, int.MaxValue);
        Y = Math.Clamp(y, 0, int.MaxValue);
    }

    /// <summary>
    /// <see langword="true"/> if this <see cref="NexusCoord"/> is in range of <paramref name="start"/> and <paramref name="end"/>
    /// </summary>
    /// <param name="start">The start coordinate</param>
    /// <param name="end">The end coordinate</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsInRange(in NexusCoord start, in NexusCoord end)
        => X >= start.X && Y >= start.Y && X <= end.X && Y <= end.Y;

    /// <summary>
    /// <see langword="true"/> if this <see cref="NexusCoord"/> is in range of <paramref name="start"/> and <paramref name="range"/>
    /// </summary>
    /// <param name="start">The start coordinate</param>
    /// <param name="range">The range size</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsInRange(in NexusCoord start, in NexusSize range)
        => X >= start.X && Y >= start.Y && X <= start.X + range.Width && Y <= start.Y + range.Height;

    /// <summary>
    /// Converts this object to a <see cref="NexusSize"/>
    /// </summary>
    /// <returns><see cref="NexusSize"/></returns>
    public readonly NexusSize ToSize() => new NexusSize(X, Y);

    /// <inheritdoc/>
    public readonly void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    /// <inheritdoc/>
    public static NexusCoord operator +(NexusCoord left, NexusCoord right)
        => new(left.X + right.X, left.Y + right.Y);

    /// <inheritdoc/>
    public static NexusCoord operator -(NexusCoord left, NexusCoord right)
        => new(left.X - right.X, left.Y - right.Y);

    /// <inheritdoc/>
    public static NexusCoord operator *(NexusCoord left, NexusCoord right)
        => new(left.X * right.X, left.Y * right.Y);

    /// <inheritdoc/>
    public static NexusCoord operator /(NexusCoord left, NexusCoord right)
        => new(left.X / right.X, left.Y / right.Y);

    /// <inheritdoc/>
    public static NexusCoord operator --(NexusCoord value)
        => new(value.X - 1, value.Y - 1);

    /// <inheritdoc/>
    public static NexusCoord operator ++(NexusCoord value)
        => new(value.X + 1, value.Y + 1);

    /// <inheritdoc/>
    public override readonly string ToString() => $"[{X},{Y}]";
}