namespace ConsoleNexusEngine.Sound;

using System.Numerics;

/// <summary>
/// Represents volume between 0 - 100
/// </summary>
public readonly record struct NexusVolume
    : IAdditionOperators<NexusVolume, NexusVolume, NexusVolume>,
    ISubtractionOperators<NexusVolume, NexusVolume, NexusVolume>,
    IMultiplyOperators<NexusVolume, NexusVolume, NexusVolume>,
    IDivisionOperators<NexusVolume, NexusVolume, NexusVolume>,
    IMinMaxValue<NexusVolume>,
    IDecrementOperators<NexusVolume>,
    IIncrementOperators<NexusVolume>
{
    /// <inheritdoc/>
    public static NexusVolume MinValue => (NexusVolume)0;
    /// <inheritdoc/>
    public static NexusVolume MaxValue => (NexusVolume)100;

    internal readonly float _value;

    /// <summary>
    /// The volume between 0 and 100
    /// </summary>
    /// <remarks>Value is clamped between 0 and 100</remarks>
    public readonly int Value => (int)(_value * 100);

    /// <summary>
    /// Returns <see langword="true"/> if the volume is muted, otherwise <see langword="false"/>
    /// </summary>
    public readonly bool IsMute => Value is 0;

    /// <summary>
    /// Initializes a Volume instance with <see cref="Value"/> 50
    /// </summary>
    public NexusVolume() : this(0.5f) { }

    /// <summary>
    /// Initializes a Volume instance
    /// </summary>
    public NexusVolume(in int volume) : this(volume / 100f) { }

    /// <summary>
    /// Initializes a Volume instance
    /// </summary>
    public NexusVolume(in float volume) => _value = Math.Clamp(volume, 0, 1);

    /// <inheritdoc/>
    public static explicit operator int(in NexusVolume volume) => volume.Value;

    /// <inheritdoc/>
    public static explicit operator NexusVolume(in int volume) => new(volume);

    /// <inheritdoc/>
    public static explicit operator float(in NexusVolume volume) => volume._value;

    /// <inheritdoc/>
    public static explicit operator NexusVolume(in float volume) => new(volume);

    /// <inheritdoc/>
    public static NexusVolume operator +(NexusVolume left, NexusVolume right)
        => new(left._value + right._value);

    /// <inheritdoc/>
    public static NexusVolume operator -(NexusVolume left, NexusVolume right)
        => new(left._value - right._value);

    /// <inheritdoc/>
    public static NexusVolume operator *(NexusVolume left, NexusVolume right)
        => new(left._value * right._value);

    /// <inheritdoc/>
    public static NexusVolume operator /(NexusVolume left, NexusVolume right)
        => new(left._value / right._value);

    /// <inheritdoc/>
    public static NexusVolume operator ++(NexusVolume value) => new(value._value + 0.01f);

    /// <inheritdoc/>
    public static NexusVolume operator --(NexusVolume value) => new(value._value - 0.01f);

    /// <inheritdoc/>
    public override readonly string ToString() => Value.ToString();
}