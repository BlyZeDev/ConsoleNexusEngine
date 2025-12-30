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
            Usage = WasapiUsage.ProAudio
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
    private readonly Dictionary<NexusAudioId, PlayingSoundInfo> _playingAudio;

    /// <summary>
    /// Enumeration of all available playback audio devices
    /// </summary>
    public IEnumerable<NexusAudioDevice> AudioDevices
    {
        get
        {
            _audioEngine.UpdateAudioDevicesInfo();

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

        var player = new SurroundPlayer(_audioEngine, _audioFormat, new ChunkedDataProvider(_audioEngine, _audioFormat, File.OpenRead(filepath)));
        player.SetSpeakerConfiguration(SurroundPlayer.SpeakerConfiguration.Stereo);

        playbackDevice.MasterMixer.AddComponent(player);
        playbackDevice.Start();

        var audioId = new NexusAudioId(Environment.TickCount64);

        player.PlaybackEnded += OnPlaybackEndedLocal;
        player.Play();

        _playingAudio.Add(audioId, new PlayingSoundInfo
        {
            DeviceInfo = device._deviceInfo,
            Player = player
        });

        return audioId;

        void OnPlaybackEndedLocal(object? sender, EventArgs args)
        {
            OnPlaybackEnded(audioId);
            player.PlaybackEnded -= OnPlaybackEndedLocal;
        }
    }

    /// <summary>
    /// Pauses the specified sound
    /// </summary>
    /// <param name="id">The id of the sound</param>
    /// <remarks>
    /// This method has no effect if no sound is found or the sound is already paused
    /// </remarks>
    public void Pause(NexusAudioId id)
    {
        if (_playingAudio.TryGetValue(id, out var soundInfo)) soundInfo.Player.Pause();
    }

    /// <summary>
    /// Resumes the specified sound
    /// </summary>
    /// <param name="id">The id of the sound</param>
    /// <remarks>
    /// This method has no effect if no sound is found or the sound is already playing
    /// </remarks>
    public void Resume(NexusAudioId id)
    {
        if (_playingAudio.TryGetValue(id, out var soundInfo))
        {
            if (soundInfo.Player.State is PlaybackState.Paused) soundInfo.Player.Play();
        }
    }

    /// <summary>
    /// Restarts the specified sound
    /// </summary>
    /// <param name="id">The id of the sound</param>
    /// <remarks>
    /// This method has no effect if no sound is found
    /// </remarks>
    public void Restart(NexusAudioId id)
    {
        if (_playingAudio.TryGetValue(id, out var soundInfo))
        {
            soundInfo.Player.Stop();
            soundInfo.Player.Play();
        }
    }

    /// <summary>
    /// Stops and removes the specified sound
    /// </summary>
    /// <param name="id">The id of the sound</param>
    /// <remarks>
    /// The <see cref="NexusAudioId"/> is deprecated after calling this method.<br/>
    /// If you want to restart the sound use <see cref="Restart"/>
    /// </remarks>
    public void Stop(NexusAudioId id) => OnPlaybackEnded(id);

    /// <summary>
    /// Returns the current state of the specified sound
    /// </summary>
    /// <param name="id">The id of the sound</param>
    /// <remarks>
    /// Returns <see cref="NexusAudioState.Empty"/> if no sound is found.<br/>
    /// To ensure a valid state was returned use <see cref="NexusAudioState.IsEmpty"/>
    /// </remarks>
    /// <returns><see cref="NexusAudioState"/></returns>
    public NexusAudioState GetState(NexusAudioId id)
    {
        if (_playingAudio.TryGetValue(id, out var soundInfo))
        {
            var player = soundInfo.Player;

            return new NexusAudioState(
                TimeSpan.FromSeconds(player.Duration),
                TimeSpan.FromSeconds(player.Time),
                player.Volume,
                player.State is PlaybackState.Paused ? NexusPlaybackState.Paused : NexusPlaybackState.Playing,
                player.IsLooping,
                player.PlaybackSpeed);
        }

        return NexusAudioState.Empty;
    }

    /// <summary>
    /// Sets the volume for the specified sound
    /// </summary>
    /// <param name="id">The id of the sound</param>
    /// <param name="volume">The volume to set the sound to</param>
    /// <remarks>
    /// This method has no effect if no sound is found
    /// </remarks>
    public void SetVolume(NexusAudioId id, float volume)
    {
        if (_playingAudio.TryGetValue(id, out var soundInfo)) soundInfo.Player.Volume = volume;
    }

    /// <summary>
    /// Sets the playback speed for the specified sound
    /// </summary>
    /// <param name="id">The id of the sound</param>
    /// <param name="playbackSpeed">The normal speed is 1.0f, lower values decrease the speed and higher values increase it</param>
    /// <remarks>
    /// This method has no effect if no sound is found
    /// </remarks>
    public void SetPlaybackSpeed(NexusAudioId id, float playbackSpeed)
    {
        if (_playingAudio.TryGetValue(id, out var soundInfo)) soundInfo.Player.PlaybackSpeed = playbackSpeed;
    }

    /// <summary>
    /// Seeks the specified sound to the specified position
    /// </summary>
    /// <param name="id">The id of the sound</param>
    /// <param name="position">The position to seek to</param>
    /// <remarks>
    /// Returns <see langword="true"/> if the sound was found and seeking was successful, otherwise <see langword="false"/>
    /// </remarks>
    /// <returns><see cref="bool"/></returns>
    public bool Seek(NexusAudioId id, in TimeSpan position) => _playingAudio.TryGetValue(id, out var soundInfo) && soundInfo.Player.Seek(position);

    /// <summary>
    /// Turns the loop for the specified sound on or off
    /// </summary>
    /// <param name="id">The id of the sound</param>
    /// <param name="shouldLoop"><see langword="true"/> if the sound should loop, otherwise <see langword="false"/></param>
    /// <param name="startTime">The time where the loop should start, or <see langword="null"/> if the natural start of the sound should be used</param>
    /// <param name="endTime">The time where the loop should end, or <see langword="null"/> if the natural end of the sound should be used</param>
    public void SetLoop(NexusAudioId id, bool shouldLoop, TimeSpan? startTime = null, TimeSpan? endTime = null)
    {
        if (_playingAudio.TryGetValue(id, out var soundInfo))
        {
            soundInfo.Player.IsLooping = shouldLoop;
            soundInfo.Player.SetLoopPoints(startTime ?? TimeSpan.Zero, endTime);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        foreach (var soundInfo in _playingAudio.Values) soundInfo.Player.Dispose();
        foreach (var playbackDevice in _playbackDevices.Values) playbackDevice.Dispose();

        _audioEngine.Dispose();
        GC.SuppressFinalize(this);
    }

    private void OnPlaybackEnded(NexusAudioId audioId)
    {
        if (_playingAudio.TryGetValue(audioId, out var soundInfo))
        {
            soundInfo.Player.Stop();

            if (_playbackDevices.TryGetValue(soundInfo.DeviceInfo, out var playbackDevices)) playbackDevices.MasterMixer.RemoveComponent(soundInfo.Player);
        }

        soundInfo?.Player.Dispose();
    }

    private sealed class DeviceInfoEqualityComparer : IEqualityComparer<DeviceInfo>
    {
        public bool Equals(DeviceInfo x, DeviceInfo y) => x.Id == y.Id;
        public int GetHashCode([DisallowNull] DeviceInfo obj) => obj.Id.GetHashCode();
    }
}