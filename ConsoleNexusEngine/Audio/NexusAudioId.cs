namespace ConsoleNexusEngine.Audio;

/// <summary>
/// Represents information to identify a playing audio
/// </summary>
public readonly record struct NexusAudioId
{
    /// <summary>
    /// Represents an invalid audio identifier
    /// </summary>
    public static readonly NexusAudioId Invalid = new NexusAudioId(0);

    private readonly long _id;

    /// <summary>
    /// <see langword="true"/> if the identifier is valid, otherwise <see langword="false"/>
    /// </summary>
    public bool IsValid => _id != 0;

    /// <summary>
    /// Initializes an invalid audio identifier
    /// </summary>
    public NexusAudioId() : this(0) { }

    internal NexusAudioId(long id) => _id = id;
}