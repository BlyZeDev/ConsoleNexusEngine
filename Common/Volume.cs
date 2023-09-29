namespace ConsoleNexusEngine.Common;

using System;
using System.Numerics;

/// <summary>
/// Represents volume from 0 - 100
/// </summary>
public readonly record struct Volume : IAdditionOperators<Volume, Volume, Volume>, ISubtractionOperators<Volume, Volume, Volume>, IMultiplyOperators<Volume, Volume, Volume>, IDivisionOperators<Volume, Volume, Volume>, IMinMaxValue<Volume>, IDecrementOperators<Volume>, IIncrementOperators<Volume>
{
    /// <inheritdoc/>
    public static Volume MinValue { get; }
    /// <inheritdoc/>
    public static Volume MaxValue { get; }
    
    internal readonly float _value;

    /// <summary>
    /// The volume between 0 and 100
    /// </summary>
    /// <remarks>Value is clamped between 0 and 100</remarks>
    public int Value => (int)(_value * 100);

    static Volume()
    {
        MinValue = 0;
        MaxValue = 100;
    }

    /// <summary>
    /// Initializes a Volume instance with <see cref="Value"/> 50
    /// </summary>
    public Volume() : this(0.5f) { }

    /// <summary>
    /// Initializes a Volume instance
    /// </summary>
    public Volume(int volume) : this(volume / 100f) { }

    /// <summary>
    /// Initializes a Volume instance
    /// </summary>
    public Volume(float volume) => _value = Math.Clamp(volume, 0, 1);

    /// <inheritdoc/>
    public static implicit operator int(Volume volume) => volume.Value;

    /// <inheritdoc/>
    public static implicit operator Volume(int volume) => new(volume);

    /// <inheritdoc/>
    public static implicit operator float(Volume volume) => volume._value;

    /// <inheritdoc/>
    public static implicit operator Volume(float volume) => new(volume);

    /// <inheritdoc/>
    public static Volume operator +(Volume left, Volume right)
        => new(left.Value + right.Value);

    /// <inheritdoc/>
    public static Volume operator -(Volume left, Volume right)
        => new(left.Value - right.Value);

    /// <inheritdoc/>
    public static Volume operator *(Volume left, Volume right)
        => new(left.Value * right.Value);

    /// <inheritdoc/>
    public static Volume operator /(Volume left, Volume right)
        => new(left.Value / right.Value);

    /// <inheritdoc/>
    public static Volume operator ++(Volume value) => new(value.Value + 1);

    /// <inheritdoc/>
    public static Volume operator --(Volume value) => new(value.Value - 1);

    /// <inheritdoc/>
    public override string ToString() => Value.ToString();
}