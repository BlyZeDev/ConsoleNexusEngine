namespace ConsoleNexusEngine.Internal;

using NAudio.Wave;

internal sealed class LoopStream : WaveStream
{
    private readonly AudioFileReader _sourceStream;
    private readonly bool _isLooped;

    public LoopStream(AudioFileReader sourceStream, in bool isLooped)
    {
        _sourceStream = sourceStream;
        _isLooped = isLooped;
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
                if (_sourceStream.Position == 0 || !_isLooped) break;

                _sourceStream.Position = 0;
            }

            totalBytesRead += bytesRead;
        }

        return totalBytesRead;
    }
}