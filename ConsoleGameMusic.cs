namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal;
using NAudio.Wave;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Music player for a console game
/// </summary>
public sealed class ConsoleGameMusic
{
    private readonly IDictionary<NexusSound, WaveOutEvent> _playing;

    /// <summary>
    /// A collection of all sounds that are currently playing
    /// </summary>
    public IReadOnlyCollection<NexusSound> CurrentlyPlaying => _playing.Keys.AsReadOnly();

    internal ConsoleGameMusic()
    {
        _playing = new Dictionary<NexusSound, WaveOutEvent>();
    }

    /// <summary>
    /// Plays a <see cref="NexusSound"/>
    /// </summary>
    /// <param name="sound">The sound to play</param>
    public void PlaySound(NexusSound sound) //needs fix for multiple sounds
    {
        using (var reader = new LoopStream(new AudioFileReader(sound.FilePath)))
        {
            var outputDevice = new WaveOutEvent();
            var cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                outputDevice.Init(reader);
                outputDevice.Volume = sound.volume;
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    if (outputDevice.Volume != sound.volume)
                        outputDevice.Volume = sound.volume;

                    reader.EnableLoop = sound.IsLooped;
                }
            }).ConfigureAwait(false);

            _playing.Add(sound, outputDevice);
        }
    }

    /// <summary>
    /// Stops a played sound
    /// </summary>
    /// <param name="sound">The sound to stop</param>
    /// <remarks>If the sound is not found, nothing happens</remarks>
    public void StopSound(NexusSound sound)
    {
        if (!_playing.ContainsKey(sound)) return;

        _playing[sound].Stop();
        _playing[sound].Dispose();

        _playing.Remove(sound);
    }
}