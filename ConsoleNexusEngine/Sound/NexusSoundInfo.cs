namespace ConsoleNexusEngine.Sound;

using SoundFlow.Structs;

/// <summary>
/// Represents required information to play a sound
/// </summary>
public sealed record NexusSoundInfo
{
    internal readonly AudioFormat _audioFormat;

    public string FilePath { get; }

    public NexusSoundInfo(string filepath, int channels = 2)
    {
        _audioFormat = new AudioFormat
        {
            
        };
    }
}