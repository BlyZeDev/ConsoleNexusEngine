namespace ConsoleNexusEngine;

using SoundFlow.Backends.MiniAudio;
using SoundFlow.Backends.MiniAudio.Devices;
using SoundFlow.Backends.MiniAudio.Enums;

/// <summary>
/// The audio engine for <see cref="NexusConsoleGame"/>
/// </summary>
public sealed class NexusConsoleAudio : IDisposable
{
    private static readonly MiniAudioDeviceConfig _deviceConfig = new MiniAudioDeviceConfig
    {
        NoFixedSizedCallback = true,
        Wasapi = new WasapiSettings
        {
            Usage = WasapiUsage.Games
        }
    };

    private readonly MiniAudioEngine _audioEngine;

    /// <summary>
    /// Enumeration of all available playback audio devices
    /// </summary>
    public IEnumerable<NexusAudioDevice> AudioDevices
    {
        get
        {
            _audioEngine.UpdateDevicesInfo();

            foreach (var device in _audioEngine.PlaybackDevices)
            {
                yield return new NexusAudioDevice(device);
            }
        }
    }

    internal NexusConsoleAudio()
    {
        _audioEngine = new MiniAudioEngine();
    }

    public void Play(NexusAudioDevice device)
    {
        using (var dev = _audioEngine.InitializePlaybackDevice(device._deviceInfo, new SoundFlow.Structs.AudioFormat { }))
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _audioEngine.Dispose();
        GC.SuppressFinalize(this);
    }
}