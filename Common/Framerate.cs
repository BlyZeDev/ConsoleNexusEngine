namespace ConsoleNexusEngine.Common;

using System;

/// <summary>
/// Represents Frames per seconds
/// </summary>
public readonly record struct Framerate
{
    /// <summary>
    /// Frames per second
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Initializes 30 Frames per second
    /// </summary>
    public Framerate() : this(30) { }

    /// <summary>
    /// Initializes a new Framerate. Framerate can't be lower than 1
    /// </summary>
    /// <param name="framerate">Frames per second</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Framerate(int framerate)
    {
        if (framerate is < 1)
            throw new ArgumentOutOfRangeException(nameof(framerate), "The framerate can't be lower than 1");

        Value = framerate;
    }

    public static implicit operator int(Framerate framerate) => framerate.Value;

    public static implicit operator Framerate(int framerate) => new(framerate);
}