namespace ConsoleNexusEngine;

using System.Numerics;

/// <summary>
/// Represents frames per seconds
/// </summary>
public readonly record struct NexusFramerate : IMinMaxValue<NexusFramerate>
{
    /// <inheritdoc/>
    public static NexusFramerate MaxValue => new NexusFramerate(int.MaxValue);
    /// <inheritdoc/>
    public static NexusFramerate MinValue => new NexusFramerate(0);

    /// <summary>
    /// Frames per second
    /// </summary>
    public readonly int Value { get; }

    /// <summary>
    /// Initializes 60 Frames per second
    /// </summary>
    public NexusFramerate() : this(60) { }

    /// <summary>
    /// Initializes a new Framerate. Framerate can't be lower than 0
    /// </summary>
    /// <param name="framerate">Frames per second</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public NexusFramerate(int framerate)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(framerate, 0, nameof(framerate));

        Value = framerate;
    }

    /// <summary>
    /// Explicitly converts <see cref="NexusFramerate"/> to <see cref="int"/>
    /// </summary>
    /// <param name="framerate">Framerate to convert</param>
    public static explicit operator int(in NexusFramerate framerate) => framerate.Value;

    /// <summary>
    /// Implicitly converts <see cref="int"/> to <see cref="NexusFramerate"/>
    /// </summary>
    /// <param name="framerate">Framerate to convert</param>
    public static implicit operator NexusFramerate(int framerate) => new(framerate);

    /// <inheritdoc/>
    public override readonly string ToString() => Value.ToString();
}