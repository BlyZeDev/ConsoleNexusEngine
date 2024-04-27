namespace ConsoleNexusEngine;

/// <summary>
/// Represents Frames per seconds
/// </summary>
public readonly record struct NexusFramerate
{
    /// <summary>
    /// <see cref="Value"/> set to -1 which means unlimited
    /// </summary>
    public static NexusFramerate Unlimited => new(-1);

    /// <summary>
    /// Frames per second
    /// </summary>
    public readonly int Value { get; }

    /// <summary>
    /// <see langword="true"/> if <see cref="Value"/> is -1, otherwise <see langword="false"/>
    /// </summary>
    public readonly bool IsUnlimited => Value is -1;

    /// <summary>
    /// Initializes 30 Frames per second
    /// </summary>
    public NexusFramerate() : this(30) { }

    /// <summary>
    /// Initializes a new Framerate. Framerate can't be lower than -1 (unlimited)
    /// </summary>
    /// <param name="framerate">Frames per second</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public NexusFramerate(int framerate)
    {
        if (framerate < -1)
            throw new ArgumentOutOfRangeException(nameof(framerate), "The framerate can't be lower than -1 (unlimited)");

        Value = framerate;
    }

    /// <summary>
    /// Explicitly converts <see cref="NexusFramerate"/> to <see cref="int"/>
    /// </summary>
    /// <param name="framerate">Framerate to convert</param>
    public static explicit operator int(NexusFramerate framerate) => framerate.Value;

    /// <summary>
    /// Implicitly converts <see cref="int"/> to <see cref="NexusFramerate"/>
    /// </summary>
    /// <param name="framerate">Framerate to convert</param>
    public static implicit operator NexusFramerate(int framerate) => new(framerate);

    /// <inheritdoc/>
    public override readonly string ToString() => Value.ToString();
}