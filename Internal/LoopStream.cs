namespace ConsoleNexusEngine.Internal;

using ConsoleNexusEngine.Common;
using NAudio.Wave;
using System;

internal sealed class LoopStream : WaveStream
{
    private readonly AudioFileReader _sourceStream;
    private readonly NexusSound _sound;

    public LoopStream(AudioFileReader sourceStream, NexusSound sound)
    {
        _sourceStream = sourceStream;
        _sound = sound;

        _sound.OnVolumeChanged += OnVolumeChanged;
    }

    public override WaveFormat WaveFormat => _sourceStream.WaveFormat;

    public override long Length => _sourceStream.Length;

    public override long Position
    {
        get => _sourceStream.Position;
        set => _sourceStream.Position = value;
    }

    public float Volume
    {
        get => _sourceStream.Volume;
        set => _sourceStream.Volume = value;
    }
    
    public override int Read(byte[] buffer, int offset, int count)
    {
        var totalBytesRead = 0;

        while (totalBytesRead < count)
        {
            var bytesRead = _sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);

            if (bytesRead == 0)
            {
                if (_sourceStream.Position == 0 || !_sound.IsLooped) break;

                _sourceStream.Position = 0;
            }

            totalBytesRead += bytesRead;
        }

        return totalBytesRead;
    }

    private void OnVolumeChanged(object? sender, EventArgs e) => Volume = _sound.Volume._value;
}