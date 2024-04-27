namespace ConsoleNexusEngine.Sound;

using NAudio.Wave;

/// <summary>
/// Represents a sound playing in the console
/// </summary>
public sealed record NexusSound : IDisposable
{
    private readonly WaveOutEvent _wave;

    private LoopStream stream = null!;
    private bool isDisposed;

    /// <summary>
    /// The length of the sound
    /// </summary>
    public TimeSpan Length => stream.TotalTime;

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
        get => stream.CurrentTime;
        private set => stream.CurrentTime = value;
    }

    /// <summary>
    /// The volume of the sound
    /// </summary>
    /// <remarks>Clamped between 0 and 100</remarks>
    public NexusVolume Volume
    {
        get => (NexusVolume)stream.Volume;
        set => stream.Volume = value._value;
    }

    /// <summary>
    /// Initializes a new Nexus Sound
    /// </summary>
    /// <param name="filePath">The path to the file that should be played</param>
    /// <param name="volume">The volume of the played sound</param>
    /// <param name="shouldLoop"><see langword="true"/> if the sound should be looped, otherwise <see langword="false"/></param>
    public NexusSound(string filePath, NexusVolume volume, bool shouldLoop = false)
    {
        _wave = new WaveOutEvent();
        _wave.PlaybackStopped += OnFinish;

        FilePath = filePath;
        IsLooped = shouldLoop;

        State = NexusPlayerState.NotStarted;

        InitWave(volume);
    }

    /// <summary>
    /// Plays the sound
    /// </summary>
    public void Play()
    {
        if (State is NexusPlayerState.Playing) return;

        if (State is NexusPlayerState.Finished) InitWave(Volume);

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
    /// Restarts the sound
    /// </summary>
    public void Restart()
    {
        if (State is NexusPlayerState.NotStarted) return;

        _wave.Stop();

        State = NexusPlayerState.Finished;

        Play();
    }

    /// <summary>
    /// Rewinds the sound by the given period
    /// </summary>
    /// <param name="period">The period to rewind</param>
    public void Rewind(TimeSpan period) => Rewind((int)period.TotalSeconds);

    /// <summary>
    /// Rewinds the sound by the given period
    /// </summary>
    /// <param name="seconds">The period to rewind in seconds</param>
    public void Rewind(int seconds) => ChangePos(-seconds);

    /// <summary>
    /// Forwards the sound by the given period
    /// </summary>
    /// <param name="period">The period to forward</param>
    public void Forward(TimeSpan period) => Forward((int)period.TotalSeconds);

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
    public void Seek(TimeSpan position)
        => Position = TimeSpan.FromTicks(Math.Clamp(position.Ticks, TimeSpan.Zero.Ticks, Length.Ticks));

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!isDisposed)
        {
            _wave.Dispose();
            stream.Dispose();

            isDisposed = true;
        }

        GC.SuppressFinalize(this);
    }

    private void InitWave(NexusVolume volume)
    {
        stream = new LoopStream(new AudioFileReader(FilePath), IsLooped)
        {
            Volume = volume._value
        };

        _wave.Init(stream);
    }

    private void ChangePos(int seconds) => stream.Skip(seconds);

    private void OnFinish(object? sender, StoppedEventArgs e) => Stop();
}