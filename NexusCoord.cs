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

    private readonly int x;
    private readonly int y;

    /// <summary>
    /// The X coordinate
    /// </summary>
    public int X
    {
        readonly get => x;
        init => x = Math.Clamp(value, 0, int.MaxValue);
    }

    /// <summary>
    /// The Y coordinate
    /// </summary>
    public int Y
    {
        readonly get => y;
        init => y = Math.Clamp(value, 0, int.MaxValue);
    }

    /// <summary>
    /// Initializes a new <see cref="NexusCoord"/>
    /// </summary>
    public NexusCoord() : this(0, 0) { }

    /// <summary>
    /// Initializes a new <see cref="NexusCoord"/>
    /// </summary>
    /// <param name="x">The X coordinate</param>
    /// <param name="y">The Y coordinate</param>
    public NexusCoord(int x, int y)
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
    public readonly bool IsInRange(NexusCoord from, NexusCoord to)
        => X >= from.X && X <= to.X && Y >= from.Y && Y <= to.Y;

    /// <inheritdoc/>
    public readonly void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    internal readonly COORD ToCOORD() => new()
    {
        X = (short)X,
        Y = (short)Y
    };

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

    internal static NexusCoord FromCOORD(COORD coord)
        => new(coord.X, coord.Y);

    /// <inheritdoc/>
    public override readonly string ToString() => $"[{X},{Y}]";
}