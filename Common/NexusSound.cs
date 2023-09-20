namespace ConsoleNexusEngine.Common;

using System;

/// <summary>
/// Represents a sound playing in the console
/// </summary>
public sealed class NexusSound
{
    internal float volume;

    /// <summary>
    /// The path to the played file
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// <see langword="true"/> if the sound is looped, otherwise <see langword="false"/>
    /// </summary>
    public bool IsLooped { get; set; }

    /// <summary>
    /// The volume of the sound
    /// </summary>
    /// <remarks>Clamped between 0 and 100</remarks>
    public int Volume
    {
        get => (int)(volume * 100);
        set
        {
            volume = Math.Clamp(value / 100f, 0, 1);
        }
    }

    /// <summary>
    /// Initializes a new Nexus Sound
    /// </summary>
    /// <param name="filePath">The path to the file that should be played</param>
    /// <param name="volume">The volume of the played sound, [0-100]</param>
    /// /// <param name="shouldLoop"><see langword="true"/> if the sound should be looped, otherwise <see langword="false"/></param>
    public NexusSound(string filePath, int volume = 50, bool shouldLoop = false)
    {
        FilePath = filePath;
        Volume = volume;
        IsLooped = shouldLoop;
    }
}