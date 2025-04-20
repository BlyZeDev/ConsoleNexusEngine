namespace ConsoleNexusEngine.Sound;

using NAudio.Wave;

/// <summary>
/// Represents a sound playing in the console
/// </summary>
public sealed record NexusSound : IDisposable
{
    private readonly WaveOutEvent _wave;
    private readonly LoopStream _stream;

    private bool isDisposed;

    /// <summary>
    /// The length of the sound
    /// </summary>
    public TimeSpan Length => _stream.TotalTime;

    /// <summary>
    /// The path to the sound file
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// <see langword="true"/> if the sound is looped, otherwise <see langword="false"/>
    /// </summary>
    public bool IsLooped { get; }

    /// <summary>
    /// The state of the sound
    /// </summary>
    public NexusPlayerState State { get; private set; }

    /// <summary>
    /// The position of the sound
    /// </summary>
    public TimeSpan Position
    {
        get => _stream.CurrentTime;
        private set => _stream.CurrentTime = value;
    }

    /// <summary>
    /// The volume of the sound
    /// </summary>
    /// <remarks>Clamped between 0 and 100</remarks>
    public NexusVolume Volume
    {
        get => (NexusVolume)_stream.Volume;
        set => _stream.Volume = value._value;
    }

    /// <summary>
    /// Initializes a new Nexus Sound
    /// </summary>
    /// <param name="filePath">The path to the file that should be played</param>
    /// <param name="volume">The volume of the played sound</param>
    /// <param name="shouldLoop"><see langword="true"/> if the sound should be looped, otherwise <see langword="false"/></param>
    public NexusSound(string filePath, in NexusVolume volume, bool shouldLoop = false)
    {
        _wave = new WaveOutEvent()
        {
            DesiredLatency = 100
        };
        _wave.PlaybackStopped += OnFinish;

        _stream = new LoopStream(new AudioFileReader(filePath), shouldLoop);
        _wave.Init(_stream);

        FilePath = filePath;
        IsLooped = shouldLoop;
        Volume = volume;

        State = NexusPlayerState.NotStarted;
    }

    /// <summary>
    /// Plays the sound
    /// </summary>
    /// <param name="forceRestart"><see langword="true"/> if the sound should restart if it's already playing</param>
    public void Play(bool forceRestart = false)
    {
        if (forceRestart)
        {
            _wave.Stop();
            State = NexusPlayerState.Finished;
        }
        else
        {
            if (State is NexusPlayerState.Playing) return;
        }

        if (State is NexusPlayerState.Finished) _stream.Position = 0;

        _wave.Play();
        State = NexusPlayerState.Playing;
    }

    /// <summary>
    /// Pauses the sound
    /// </summary>
    public void Pause()
    {
        if (State is not NexusPlayerState.Playing) return;

        _wave.Pause();
        State = NexusPlayerState.Paused;
    }

    /// <summary>
    /// Stops the sound
    /// </summary>
    public void Stop()
    {
        if (State is NexusPlayerState.NotStarted or NexusPlayerState.Finished) return;

        _wave.Stop();
        State = NexusPlayerState.Finished;
    }

    /// <summary>
    /// Rewinds the sound by the given period
    /// </summary>
    /// <param name="period">The period to rewind</param>
    public void Rewind(in TimeSpan period) => Rewind((int)period.TotalSeconds);

    /// <summary>
    /// Rewinds the sound by the given period
    /// </summary>
    /// <param name="seconds">The period to rewind in seconds</param>
    public void Rewind(int seconds) => ChangePos(-seconds);

    /// <summary>
    /// Forwards the sound by the given period
    /// </summary>
    /// <param name="period">The period to forward</param>
    public void Forward(in TimeSpan period) => Forward((int)period.TotalSeconds);

    /// <summary>
    /// Forwards the sound by the given period
    /// </summary>
    /// <param name="seconds">The period to forward in seconds</param>
    public void Forward(int seconds) => ChangePos(seconds);

    /// <summary>
    /// Jumps to a specific position
    /// </summary>
    /// <remarks><paramref name="position"/> is clamped to be in range of the sound</remarks>
    /// <param name="position">The position to jump to</param>
    public void Seek(in TimeSpan position)
        => Position = TimeSpan.FromTicks(Math.Clamp(position.Ticks, TimeSpan.Zero.Ticks, Length.Ticks));

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!isDisposed)
        {
            _wave.Dispose();
            _stream.Dispose();

            isDisposed = true;
        }

        GC.SuppressFinalize(this);
    }

    private void ChangePos(int seconds) => _stream.Skip(seconds);

    private void OnFinish(object? sender, StoppedEventArgs e) => Stop();
}