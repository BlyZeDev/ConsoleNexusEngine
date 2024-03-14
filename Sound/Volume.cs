namespace ConsoleNexusEngine.Sound;

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

    /// <summary>
    /// Returns <see langword="true"/> if the volume is muted, otherwise <see langword="false"/>
    /// </summary>
    public bool IsMute => Value is 0;

    static Volume()
    {
        MinValue = (Volume)0;
        MaxValue = (Volume)100;
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
    public static explicit operator int(Volume volume) => volume.Value;

    /// <inheritdoc/>
    public static explicit operator Volume(int volume) => new(volume);

    /// <inheritdoc/>
    public static explicit operator float(Volume volume) => volume._value;

    /// <inheritdoc/>
    public static explicit operator Volume(float volume) => new(volume);

    /// <inheritdoc/>
    public static Volume operator +(Volume left, Volume right)
        => new(left._value + right._value);

    /// <inheritdoc/>
    public static Volume operator -(Volume left, Volume right)
        => new(left._value - right._value);

    /// <inheritdoc/>
    public static Volume operator *(Volume left, Volume right)
        => new(left._value * right._value);

    /// <inheritdoc/>
    public static Volume operator /(Volume left, Volume right)
        => new(left._value / right._value);

    /// <inheritdoc/>
    public static Volume operator ++(Volume value) => new(value._value + 0.01f);

    /// <inheritdoc/>
    public static Volume operator --(Volume value) => new(value._value - 0.01f);

    /// <inheritdoc/>
    public override string ToString() => Value.ToString();
}