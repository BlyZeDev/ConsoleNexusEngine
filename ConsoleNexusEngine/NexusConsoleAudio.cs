namespace ConsoleNexusEngine;

using SoundFlow.Backends.MiniAudio;
using SoundFlow.Backends.MiniAudio.Devices;
using SoundFlow.Backends.MiniAudio.Enums;
using SoundFlow.Components;
using SoundFlow.Providers;
using SoundFlow.Structs;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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

    public void Play(NexusSoundInfo soundInfo)
    {
        var defaultDevice = AudioDevices.FirstOrDefault(x => x.IsDefault);

        if (defaultDevice is null) return;

        Play(defaultDevice, soundInfo);
    }

    public void Play(NexusAudioDevice device, NexusSoundInfo soundInfo)
    {
        if (!File.Exists(soundInfo.FilePath)) return;

        var playbackDevice = _audioEngine.InitializePlaybackDevice(device._deviceInfo, soundInfo._audioFormat, _deviceConfig);
        var dataProvider = new StreamDataProvider(_audioEngine, soundInfo._audioFormat, File.OpenRead(soundInfo.FilePath));
        var player = new SoundPlayer(_audioEngine, soundInfo._audioFormat, dataProvider);

        playbackDevice.MasterMixer.AddComponent(player);
        playbackDevice.Start();
        player.Play();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _audioEngine.Dispose();
        GC.SuppressFinalize(this);
    }
}