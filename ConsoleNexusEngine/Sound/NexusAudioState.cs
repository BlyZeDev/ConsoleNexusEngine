namespace ConsoleNexusEngine.Sound;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents the current state of a playing sound
/// </summary>
public readonly record struct NexusAudioState
{
    /// <summary>
    /// Represents an empty audio state
    /// </summary>
    public static readonly NexusAudioState Empty = new NexusAudioState(true);

    /// <summary>
    /// The total length of the sound
    /// </summary>
    public required TimeSpan Duration { get; init; }

    /// <summary>
    /// The current position of the sound
    /// </summary>
    public required TimeSpan Position { get; init; }

    /// <summary>
    /// The volume of the sound
    /// </summary>
    public required NexusVolume Volume { get; init; }

    /// <summary>
    /// The current playback state of the sound
    /// </summary>
    public required NexusPlaybackState PlaybackState { get; init; }

    /// <summary>
    /// <see langword="true"/> if the sound is currently looping, otherwise <see langword="false"/>
    /// </summary>
    public required bool IsLooping { get; init; }

    /// <summary>
    /// The playback speed of the sound
    /// </summary>
    /// <remarks>
    /// The normal speed is 1.0f, lower values decrease the speed and higher values increase it
    /// </remarks>
    public required float PlaybackSpeed { get; init; }

    /// <summary>
    /// <see langword="true"/> if the state is empty, otherwise <see langword="false"/>
    /// </summary>
    public bool IsEmpty { get; }

    [SetsRequiredMembers]
    private NexusAudioState(bool isEmpty) => IsEmpty = isEmpty;
}