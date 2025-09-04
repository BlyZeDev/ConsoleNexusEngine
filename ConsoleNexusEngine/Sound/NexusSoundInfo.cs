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
    /// <param name="sampleFormat">The sample format</param>
    /// <param name="sampleRate">The sample rate in Hertz</param>
    /// <param name="channels">The amount of channels used. 1 = mono, 2 = stereo</param>
    public NexusSoundInfo(string filepath, SampleFormat sampleFormat = SampleFormat.F32, int sampleRate = 48000, int channels = 2)
    {
        _audioFormat = new AudioFormat
        {
            Format = sampleFormat,
            SampleRate = sampleRate,
            Channels = channels,
        };

        FilePath = filepath;
    }
}