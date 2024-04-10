namespace ConsoleNexusEngine;

using System;

/// <summary>
/// Represents Frames per seconds
/// </summary>
public readonly record struct Framerate
{
    /// <summary>
    /// <see cref="Value"/> set to -1 which means unlimited
    /// </summary>
    public static Framerate Unlimited { get; }

    /// <summary>
    /// Frames per second
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// <see langword="true"/> if <see cref="Value"/> is -1, otherwise <see langword="false"/>
    /// </summary>
    public bool IsUnlimited => Value is -1;

    static Framerate() => Unlimited = new(-1);

    /// <summary>
    /// Initializes 30 Frames per second
    /// </summary>
    public Framerate() : this(30) { }

    /// <summary>
    /// Initializes a new Framerate. Framerate can't be lower than 0
    /// </summary>
    /// <param name="framerate">Frames per second</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Framerate(int framerate)
    {
        if (framerate is < -1)
            throw new ArgumentOutOfRangeException(nameof(framerate), "The framerate can't be lower than -1 (unlimited)");

        Value = framerate;
    }

    /// <summary>
    /// Explicitly converts <see cref="Framerate"/> to <see cref="int"/>
    /// </summary>
    /// <param name="framerate">Framerate to convert</param>
    public static explicit operator int(Framerate framerate) => framerate.Value;

    /// <summary>
    /// Implicitly converts <see cref="int"/> to <see cref="Framerate"/>
    /// </summary>
    /// <param name="framerate">Framerate to convert</param>
    public static implicit operator Framerate(int framerate) => new(framerate);

    /// <inheritdoc/>
    public override string ToString() => Value.ToString();
}