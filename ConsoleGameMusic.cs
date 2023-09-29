namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal;
using NAudio.Wave;
using System.Collections.Generic;

/// <summary>
/// Music player for a console game
/// </summary>
public sealed class ConsoleGameMusic
{
    private readonly IDictionary<NexusSound, WaveOutEvent> _playing;

    /// <summary>
    /// The sounds that are currently played
    /// </summary>
    public IReadOnlyCollection<NexusSound> CurrentlyPlaying => _playing.Keys.AsReadOnly();

    internal ConsoleGameMusic()
    {
        _playing = new Dictionary<NexusSound, WaveOutEvent>();
    }

    /// <summary>
    /// Plays a sound
    /// </summary>
    /// <param name="sound">The sound to play</param>
    public void PlaySound(NexusSound sound)
    {
        using (var reader = new LoopStream(new AudioFileReader(sound.FilePath), sound))
        {
            var outputDevice = new WaveOutEvent();

            reader.Volume = sound.Volume;

            outputDevice.PlaybackStopped += OnSoundFinished;
            outputDevice.Init(reader);
            outputDevice.Play();

            _playing.Add(sound, outputDevice);
        }
    }

    /// <summary>
    /// Pauses a playing sound
    /// </summary>
    /// <param name="sound">The sound to pause</param>
    /// <remarks>This method will do nothing if the sound is not found</remarks>
    public void PauseSound(NexusSound sound)
    {
        if (!_playing.ContainsKey(sound)) return;

        _playing[sound].Pause();
    }

    /// <summary>
    /// Resumes a playing sound
    /// </summary>
    /// <param name="sound">The sound to resume</param>
    /// <remarks>This method will do nothing if the sound is not found</remarks>
    public void ResumeSound(NexusSound sound)
    {
        if (!_playing.ContainsKey(sound)) return;

        _playing[sound].Play();
    }

    /// <summary>
    /// Stops a playing sound
    /// </summary>
    /// <param name="sound">The sound to stop</param>
    /// <remarks>This method will do nothing if the sound is not found</remarks>
    public void StopSound(NexusSound sound)
    {
        if (!_playing.ContainsKey(sound)) return;

        _playing[sound].Stop();
        _playing[sound].Dispose();

        _playing.Remove(sound);
    }

    private void OnSoundFinished(object? sender, StoppedEventArgs e)
    {
        if (!_playing.TryGetKey(sender as WaveOutEvent, out var sound)) return;

        StopSound(sound);
    }
}