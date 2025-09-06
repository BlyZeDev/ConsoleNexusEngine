namespace ConsoleNexusEngine;

using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Backends.MiniAudio.Devices;
using SoundFlow.Backends.MiniAudio.Enums;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;
using SoundFlow.Structs;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

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
    private static readonly AudioFormat _audioFormat = new AudioFormat
    {
        Channels = 2,
        Format = SampleFormat.F32,
        SampleRate = 48000
    };

    private readonly MiniAudioEngine _audioEngine;
    private readonly Dictionary<DeviceInfo, AudioPlaybackDevice> _playbackDevices;
    private readonly Dictionary<NexusAudioId, SoundPlayer> _playingAudio;

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
        _playbackDevices = new Dictionary<DeviceInfo, AudioPlaybackDevice>(new DeviceInfoEqualityComparer());
        _playingAudio = [];
    }

    /// <summary>
    /// Plays audio from a file
    /// </summary>
    /// <param name="filepath">The path to the audio file</param>
    /// <remarks>
    /// Returns a valid <see cref="NexusAudioId"/> if playback is successful, otherwise <see cref="NexusAudioId.Invalid"/>
    /// </remarks>
    /// <returns><see cref="NexusAudioId"/></returns>
    public NexusAudioId Play(string filepath)
    {
        var defaultDevice = AudioDevices.FirstOrDefault(x => x.IsDefault);

        return defaultDevice is null ? NexusAudioId.Invalid : Play(defaultDevice, filepath);
    }

    /// <summary>
    /// Plays audio from a file with the specified audio device
    /// </summary>
    /// <param name="device">The audio device to play on</param>
    /// <param name="filepath">The path to the audio file</param>
    /// <remarks>
    /// Returns a valid <see cref="NexusAudioId"/> if playback is successful, otherwise <see cref="NexusAudioId.Invalid"/>
    /// </remarks>
    /// <returns><see cref="NexusAudioId"/></returns>
    public NexusAudioId Play(NexusAudioDevice device, string filepath)
    {
        if (!File.Exists(filepath)) return NexusAudioId.Invalid;

        if (!_playbackDevices.TryGetValue(device._deviceInfo, out var playbackDevice))
        {
            playbackDevice = _audioEngine.InitializePlaybackDevice(device._deviceInfo, _audioFormat, _deviceConfig);

            if (!playbackDevice.Capability.HasFlag(Capability.Playback))
            {
                playbackDevice.Dispose();
                return NexusAudioId.Invalid;
            }

            _playbackDevices.Add(device._deviceInfo, playbackDevice);
        }

        var player = new SoundPlayer(_audioEngine, _audioFormat, new StreamDataProvider(_audioEngine, _audioFormat, File.OpenRead(filepath)));

        playbackDevice.MasterMixer.AddComponent(player);
        playbackDevice.Start();

        var audioId = new NexusAudioId(Environment.TickCount64);

        player.PlaybackEnded += OnPlaybackEndedLocal;
        player.Play();

        _playingAudio.Add(audioId, player);

        return audioId;

        void OnPlaybackEndedLocal(object? sender, EventArgs args)
        {
            OnPlaybackEnded(device._deviceInfo, audioId);
            player.PlaybackEnded -= OnPlaybackEndedLocal;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        foreach (var player in _playingAudio.Values) player.Dispose();
        foreach (var playbackDevice in _playbackDevices.Values) playbackDevice.Dispose();

        _audioEngine.Dispose();
        GC.SuppressFinalize(this);
    }

    private void OnPlaybackEnded(DeviceInfo deviceInfo, NexusAudioId audioId)
    {
        if (_playingAudio.TryGetValue(audioId, out var player))
        {
            player.Stop();
        }

        if (player is not null && _playbackDevices.TryGetValue(deviceInfo, out var playbackDevices))
        {
            playbackDevices.MasterMixer.RemoveComponent(player);
        }

        player?.Dispose();
    }

    private sealed class DeviceInfoEqualityComparer : IEqualityComparer<DeviceInfo>
    {
        public bool Equals(DeviceInfo x, DeviceInfo y) => x.Id == y.Id;
        public int GetHashCode([DisallowNull] DeviceInfo obj) => obj.Id.GetHashCode();
    }
}