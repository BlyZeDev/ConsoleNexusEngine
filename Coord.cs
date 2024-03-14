namespace ConsoleNexusEngine;

using System;
using System.Numerics;

/// <summary>
/// Represents a positive coordinate
/// </summary>
public readonly record struct Coord
    : IAdditionOperators<Coord, Coord, Coord>,
    ISubtractionOperators<Coord, Coord, Coord>,
    IMultiplyOperators<Coord, Coord, Coord>,
    IDivisionOperators<Coord, Coord, Coord>,
    IMinMaxValue<Coord>,
    IDecrementOperators<Coord>,
    IIncrementOperators<Coord>
{
    /// <inheritdoc/>
    public static Coord MinValue { get; }

    /// <inheritdoc/>
    public static Coord MaxValue { get; }

    private readonly int x;
    private readonly int y;

    /// <summary>
    /// The X coordinate
    /// </summary>
    public int X
    {
        get => x;
        init => x = Math.Clamp(value, 0, int.MaxValue);
    }

    /// <summary>
    /// The Y coordinate
    /// </summary>
    public int Y
    {
        get => y;
        init => y = Math.Clamp(value, 0, int.MaxValue);
    }

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

    /// <summary>
    /// Returns <see langword="true"/> if the coordinate is in range of <paramref name="from"/> and <paramref name="to"/>, otherwise <see langword="false"/>
    /// </summary>
    /// <param name="from">The coordinate where the range starts</param>
    /// <param name="to">The coordinate where the range ends</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsInRange(Coord from, Coord to)
        => X >= from.X && X <= to.X && Y >= from.Y && Y <= to.Y;

    /// <inheritdoc/>
    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    internal COORD ToCOORD() => new()
    {
        X = (short)X,
        Y = (short)Y
    };

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

    internal static Coord FromCOORD(COORD coord)
        => new(coord.X, coord.Y);

    /// <inheritdoc/>
    public override string ToString() => $"[{X},{Y}]";
}