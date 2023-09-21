namespace ConsoleNexusEngine.Common;

using System;

/// <summary>
/// Represents a sound playing in the console
/// </summary>
public sealed record NexusSound
{
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
    public Volume Volume { get; set; }

    /// <summary>
    /// Initializes a new Nexus Sound
    /// </summary>
    /// <param name="filePath">The path to the file that should be played</param>
    /// <param name="volume">The volume of the played sound</param>
    /// /// <param name="shouldLoop"><see langword="true"/> if the sound should be looped, otherwise <see langword="false"/></param>
    public NexusSound(string filePath, Volume volume, bool shouldLoop = false)
    {
        FilePath = filePath;
        Volume = volume;
        IsLooped = shouldLoop;
    }
}