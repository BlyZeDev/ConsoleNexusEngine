namespace ConsoleNexusEngine.Common;

using System;
using System.Numerics;

/// <summary>
/// Represents volume from 0 - 100
/// </summary>
public readonly record struct Volume : IAdditionOperators<Volume, Volume, Volume>, ISubtractionOperators<Volume, Volume, Volume>, IMultiplyOperators<Volume, Volume, Volume>, IDivisionOperators<Volume, Volume, Volume>, IMinMaxValue<Volume>, IDecrementOperators<Volume>, IIncrementOperators<Volume>
{
    /// <summary>
    /// Volume set to 0
    /// </summary>
    public static Volume Mute => MinValue;

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
    /// Initializes a Volume instance with <see cref="Value"/> 100
    /// </summary>
    public Volume() : this(50) { }

    /// <summary>
    /// Initializes a Volume instance
    /// </summary>
    public Volume(int volume) => _value = Math.Clamp(volume, 0, 100) / 100f;

    /// <inheritdoc/>
    public static implicit operator int(Volume volume) => volume.Value;
    /// <inheritdoc/>
    public static implicit operator Volume(int volume) => new(volume);

    /// <inheritdoc/>
    public static Volume operator +(Volume left, Volume right) => throw new System.NotImplementedException();
    /// <inheritdoc/>
    public static Volume operator -(Volume left, Volume right) => throw new System.NotImplementedException();
    /// <inheritdoc/>
    public static Volume operator *(Volume left, Volume right) => throw new System.NotImplementedException();
    /// <inheritdoc/>
    public static Volume operator /(Volume left, Volume right) => throw new System.NotImplementedException();

    /// <inheritdoc/>
    public static Volume operator ++(Volume value) => throw new System.NotImplementedException();
    /// <inheritdoc/>
    public static Volume operator --(Volume value) => throw new System.NotImplementedException();
}