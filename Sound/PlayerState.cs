namespace ConsoleNexusEngine.Sound;

/// <summary>
/// The state of a <see cref="NexusSound"/>
/// </summary>
public enum PlayerState
{
    /// <summary>
    /// The sound has not started
    /// </summary>
    NotStarted,
    /// <summary>
    /// The sound is playing
    /// </summary>
    Playing,
    /// <summary>
    /// The sound is paused
    /// </summary>
    Paused,
    /// <summary>
    /// The sound has finished playing
    /// </summary>
    Finished
}