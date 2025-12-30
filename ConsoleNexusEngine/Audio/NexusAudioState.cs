namespace ConsoleNexusEngine.Audio;

/// <summary>
/// Represents the current state of a playing sound
/// </summary>
public readonly record struct NexusAudioState
{
    /// <summary>
    /// Represents an empty audio state
    /// </summary>
    public static readonly NexusAudioState Empty = new NexusAudioState();

    /// <summary>
    /// The total length of the sound
    /// </summary>
    public TimeSpan Duration { get; }

    /// <summary>
    /// The current position of the sound
    /// </summary>
    public TimeSpan Position { get; }

    /// <summary>
    /// The volume of the sound
    /// </summary>
    public float Volume { get; }

    /// <summary>
    /// The current playback state of the sound
    /// </summary>
    public NexusPlaybackState PlaybackState { get; }

    /// <summary>
    /// <see langword="true"/> if the sound is currently looping, otherwise <see langword="false"/>
    /// </summary>
    public bool IsLooping { get; }

    /// <summary>
    /// The playback speed of the sound
    /// </summary>
    /// <remarks>
    /// The normal speed is 1.0f, lower values decrease the speed and higher values increase it
    /// </remarks>
    public float PlaybackSpeed { get; }

    /// <summary>
    /// <see langword="true"/> if the state is empty, otherwise <see langword="false"/>
    /// </summary>
    public bool IsEmpty { get; }

    /// <summary>
    /// Initializes an empty <see cref="NexusAudioState"/>
    /// </summary>
    public NexusAudioState() => IsEmpty = true;

    internal NexusAudioState(TimeSpan duration, TimeSpan position, float volume, NexusPlaybackState playbackState, bool isLooping, float playbackSpeed)
    {
        IsEmpty = false;

        Duration = duration;
        Position = position;
        Volume = volume;
        PlaybackState = playbackState;
        IsLooping = isLooping;
        PlaybackSpeed = playbackSpeed;
    }
}