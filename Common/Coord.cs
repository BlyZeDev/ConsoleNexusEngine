namespace ConsoleNexusEngine.Common;

using System;
using System.Numerics;

/// <summary>
/// Represents a positive coordinate
/// </summary>
public readonly record struct Coord : IAdditionOperators<Coord, Coord, Coord>, ISubtractionOperators<Coord, Coord, Coord>, IMultiplyOperators<Coord, Coord, Coord>, IDivisionOperators<Coord, Coord, Coord>, IMinMaxValue<Coord>, IDecrementOperators<Coord>, IIncrementOperators<Coord>
{
    /// <inheritdoc/>
    public static Coord MinValue { get; }

    /// <inheritdoc/>
    public static Coord MaxValue { get; }

    /// <summary>
    /// The X coordinate
    /// </summary>
    public int X { get; }

    /// <summary>
    /// The Y coordinate
    /// </summary>
    public int Y { get; }

    static Coord()
    {
        MinValue = new Coord(0, 0);
        MaxValue = new Coord(int.MaxValue, int.MaxValue);
    }

    /// <summary>
    /// Initializes a new <see cref="Coord"/>
    /// </summary>
    public Coord() : this(0, 0) { }

    /// <summary>
    /// Initializes a new <see cref="Coord"/>
    /// </summary>
    /// <param name="x">The X coordinate</param>
    /// <param name="y">The Y coordinate</param>
    public Coord(int x, int y)
    {
        X = Math.Clamp(x, 0, int.MaxValue);
        Y = Math.Clamp(y, 0, int.MaxValue);
    }

    /// <inheritdoc/>
    public static Coord operator +(Coord left, Coord right)
        => new(left.X + right.X, left.Y + right.Y);

    /// <inheritdoc/>
    public static Coord operator -(Coord left, Coord right)
        => new(left.X - right.X, left.Y - right.Y);

    /// <inheritdoc/>
    public static Coord operator *(Coord left, Coord right)
        => new(left.X * right.X, left.Y * right.Y);

    /// <inheritdoc/>
    public static Coord operator /(Coord left, Coord right)
        => new(left.X / right.X, left.Y / right.Y);

    /// <inheritdoc/>
    public static Coord operator --(Coord value)
        => new(value.X - 1, value.Y - 1);

    /// <inheritdoc/>
    public static Coord operator ++(Coord value)
        => new(value.X + 1, value.Y + 1);
}