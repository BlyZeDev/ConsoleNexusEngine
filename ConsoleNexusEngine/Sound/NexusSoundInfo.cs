namespace ConsoleNexusEngine.Sound;

using SoundFlow.Enums;
using SoundFlow.Structs;

/// <summary>
/// Represents required information to play a sound
/// </summary>
public sealed record NexusSoundInfo
{
    internal readonly AudioFormat _audioFormat;

    /// <summary>
    /// The path to the sound file
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Initializes information to play a sound
    /// </summary>
    /// <param name="filepath">The path to the sound file</param>
    /// <param name="channels">The amount of channels used. 1 = mono, 2 = stereo</param>
    public NexusSoundInfo(string filepath, int channels = 2)
    {
        _audioFormat = new AudioFormat
        {
            Format = SampleFormat.F32,
            SampleRate = 48000,
            Channels = channels,
        };

        FilePath = filepath;
    }
}