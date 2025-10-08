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
    IAdditionOperators<NexusVolume, int, NexusVolume>,
    ISubtractionOperators<NexusVolume, int, NexusVolume>,
    IMultiplyOperators<NexusVolume, int, NexusVolume>,
    IDivisionOperators<NexusVolume, int, NexusVolume>,
    IMinMaxValue<NexusVolume>,
    IDecrementOperators<NexusVolume>,
    IIncrementOperators<NexusVolume>
{
    /// <inheritdoc/>
    public static NexusVolume MinValue => new NexusVolume(0);
    /// <inheritdoc/>
    public static NexusVolume MaxValue => new NexusVolume(200);

    internal readonly float _value;

    /// <summary>
    /// The volume between 0 and 200
    /// </summary>
    /// <remarks>Value is clamped between 0 and 200</remarks>
    public readonly int Value => (int)(_value * 100f);

    /// <summary>
    /// Returns <see langword="true"/> if the volume is muted, otherwise <see langword="false"/>
    /// </summary>
    public readonly bool IsMute => Value == 0;

    /// <summary>
    /// Initializes a <see cref="NexusVolume"/> instance with a <see cref="Value"/> of 100
    /// </summary>
    public NexusVolume() : this(100) { }

    /// <summary>
    /// Initializes a <see cref="NexusVolume"/> instance
    /// </summary>
    public NexusVolume(int volume) : this(volume / 100f) { }

    internal NexusVolume(float volume) => _value = Math.Clamp(volume, 0f, 2f);

    /// <inheritdoc/>
    public static NexusVolume operator +(NexusVolume left, NexusVolume right) => new NexusVolume(left._value + right._value);

    /// <inheritdoc/>
    public static NexusVolume operator -(NexusVolume left, NexusVolume right) => new NexusVolume(left._value - right._value);

    /// <inheritdoc/>
    public static NexusVolume operator *(NexusVolume left, NexusVolume right) => new NexusVolume(left._value * right._value);

    /// <inheritdoc/>
    public static NexusVolume operator /(NexusVolume left, NexusVolume right) => new NexusVolume(left._value / right._value);

    /// <inheritdoc/>
    public static NexusVolume operator +(NexusVolume left, int right) => new NexusVolume(left.Value + right);

    /// <inheritdoc/>
    public static NexusVolume operator -(NexusVolume left, int right) => new NexusVolume(left.Value - right);

    /// <inheritdoc/>
    public static NexusVolume operator *(NexusVolume left, int right) => new NexusVolume(left.Value * right);

    /// <inheritdoc/>
    public static NexusVolume operator /(NexusVolume left, int right) => new NexusVolume(left.Value / right);

    /// <inheritdoc/>
    public static NexusVolume operator ++(NexusVolume value) => new NexusVolume(value._value + 0.01f);

    /// <inheritdoc/>
    public static NexusVolume operator --(NexusVolume value) => new NexusVolume(value._value - 0.01f);

    /// <inheritdoc/>
    public override readonly string ToString() => Value.ToString();
}