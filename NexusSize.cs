namespace ConsoleNexusEngine;

using System.Numerics;

/// <summary>
/// Represents width and height
/// </summary>
public readonly record struct NexusSize
    : IAdditionOperators<NexusSize, NexusSize, NexusSize>,
    ISubtractionOperators<NexusSize, NexusSize, NexusSize>,
    IMultiplyOperators<NexusSize, NexusSize, NexusSize>,
    IDivisionOperators<NexusSize, NexusSize, NexusSize>,
    IMinMaxValue<NexusSize>,
    IDecrementOperators<NexusSize>,
    IIncrementOperators<NexusSize>
{
    /// <inheritdoc/>
    public static NexusSize MinValue => new(0, 0);

    /// <inheritdoc/>
    public static NexusSize MaxValue => new(int.MaxValue, int.MaxValue);

    /// <summary>
    /// The width
    /// </summary>
    public readonly int Width { get; }

    /// <summary>
    /// The height
    /// </summary>
    public readonly int Height { get; }

    /// <summary>
    /// Initializes a new <see cref="NexusSize"/>
    /// </summary>
    public NexusSize() : this(0, 0) { }

    /// <summary>
    /// Initializes a new <see cref="NexusSize"/>
    /// </summary>
    /// <remarks>
    /// The size is clamped between <see cref="int.MinValue"/> and <see cref="int.MaxValue"/>
    /// </remarks>
    /// <param name="width">The width</param>
    /// <param name="height">The height</param>
    public NexusSize(in int width, in int height)
    {
        Width = Math.Clamp(width, 0, int.MaxValue);
        Height = Math.Clamp(height, 0, int.MaxValue);
    }

    /// <summary>
    /// Initializes a new <see cref="NexusSize"/>
    /// </summary>
    /// <remarks>
    /// The size is clamped between <see cref="int.MinValue"/> and <see cref="int.MaxValue"/>
    /// </remarks>
    /// <param name="dimensions">The width and height</param>
    public NexusSize(in int dimensions) : this(dimensions, dimensions) { }

    /// <summary>
    /// Converts this object to a <see cref="NexusCoord"/>
    /// </summary>
    /// <returns><see cref="NexusCoord"/></returns>
    public readonly NexusCoord ToCoord() => new NexusCoord(Width, Height);

    /// <inheritdoc/>
    public readonly void Deconstruct(out int width, out int height)
    {
        width = Width;
        height = Height;
    }

    /// <inheritdoc/>
    public static NexusSize operator +(NexusSize left, NexusSize right)
        => new(left.Width + right.Width, left.Height + right.Height);

    /// <inheritdoc/>
    public static NexusSize operator -(NexusSize left, NexusSize right)
        => new(left.Width - right.Width, left.Height - right.Height);

    /// <inheritdoc/>
    public static NexusSize operator *(NexusSize left, NexusSize right)
        => new(left.Width * right.Width, left.Height * right.Height);

    /// <inheritdoc/>
    public static NexusSize operator /(NexusSize left, NexusSize right)
        => new(left.Width / right.Width, left.Height / right.Height);

    /// <inheritdoc/>
    public static NexusSize operator --(NexusSize value)
        => new(value.Width - 1, value.Height - 1);

    /// <inheritdoc/>
    public static NexusSize operator ++(NexusSize value)
        => new(value.Width + 1, value.Height + 1);

    /// <inheritdoc/>
    public override readonly string ToString() => $"[{Width},{Height}]";
}