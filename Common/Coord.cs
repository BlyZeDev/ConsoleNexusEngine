namespace ConsoleNexusEngine.Common;

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
    public uint X { get; }

    /// <summary>
    /// The Y coordinate
    /// </summary>
    public uint Y { get; }

    static Coord()
    {
        MinValue = new Coord(0, 0);
        MaxValue = new Coord(uint.MaxValue, uint.MaxValue);
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
    public Coord(uint x, uint y)
    {
        X = x;
        Y = y;
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