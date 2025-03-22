namespace ConsoleNexusEngine;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides methods for your Console game
/// </summary>
public abstract class NexusConsoleGame : IDisposable
{
    private static bool hasInstance;

    /// <summary>
    /// The size of the primary screen in pixels
    /// </summary>
    public static NexusSize ScreenSize { get; }

    static NexusConsoleGame()
    {
        ScreenSize = new NexusSize(Native.GetSystemMetrics(0), Native.GetSystemMetrics(1));

        hasInstance = false;
    }

    private readonly CancellationTokenSource _cts;
    private readonly CmdConsole _console;

    /// <summary>
    /// The settings of the game
    /// </summary>
    protected NexusConsoleGameSettings Settings { get; }

    /// <summary>
    /// Renders all graphics
    /// </summary>
    protected NexusConsoleGraphic Graphic { get; }

    /// <summary>
    /// Contains all input information
    /// </summary>
    protected NexusConsoleInput Input { get; }

    /// <summary>
    /// Provides useful utility functions
    /// </summary>
    protected NexusConsoleGameUtil Utility { get; }

    /// <summary>
    /// <see langword="true"/> if the game is running, otherwise <see langword="false"/>
    /// </summary>
    public bool IsRunning => !_cts.IsCancellationRequested;

    /// <summary>
    /// The total amount of rendered frames
    /// </summary>
    public uint TotalFrameCount { get; private set; }

    /// <summary>
    /// The time elapsed since the last frame in total seconds
    /// </summary>
    public double DeltaTime { get; private set; }

    /// <summary>
    /// The current FPS count
    /// </summary>
    public NexusFramerate FramesPerSecond { get; private set; }

    /// <summary>
    /// The time the game started, set in <see cref="Start()"/>
    /// </summary>
    public DateTime StartTime { get; private set; }

    /// <summary>
    /// The size of the console in characters
    /// </summary>
    public NexusSize BufferSize => new NexusSize(_console.Buffer.Width, _console.Buffer.Height);

    /// <summary>
    /// The background color of the whole console
    /// </summary>
    public NexusColor Background => Settings.ColorPalette.Color1;

    /// <summary>
    /// Initializes the <see cref="NexusConsoleGame"/>
    /// </summary>
    protected NexusConsoleGame()
    {
        if (hasInstance) throw new NexusEngineException("There is already a Console Game instance running.\nPlease dispose it before running a new Console Game");

        hasInstance = true;

        _cts = new CancellationTokenSource();
        _console = new CmdConsole(NexusConsoleGameSettings.Default);

        Settings = NexusConsoleGameSettings.Default;
        Graphic = new NexusConsoleGraphic(_console);
        Input = new NexusConsoleInput(_console);
        Utility = new NexusConsoleGameUtil(_console, Settings);

        Settings.Updated += OnSettingsUpdated;
    }

    /// <summary>
    /// Starts the game and pauses the current thread while the game is running until the <see cref="NexusConsoleGameSettings.ForceStopKey"/> is pressed
    /// </summary>
    public void Start()
    {
        if (_cts.IsCancellationRequested) throw new NexusEngineException("Can't restart a console game");

        Load();

        StartTime = DateTimeOffset.Now.DateTime;

        Task.Factory.StartNew(GameLoop, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).RunInBackground<Exception>(OnCrash, true);

        _cts.Token.WaitHandle.WaitOne();
    }

    /// <summary>
    /// Stops the game
    /// </summary>
    public void Stop()
    {
        _cts.Cancel();

        CleanUp();

        _console.ResetToDefault();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (IsRunning) Stop();

        _cts.Dispose();

        GC.SuppressFinalize(this);

        hasInstance = false;
    }

    /// <summary>
    /// Called once before the start of the game.<br/>
    /// Import game files and load resources here.
    /// </summary>
    protected abstract void Load();

    /// <summary>
    /// Called every frame
    /// </summary>
    protected abstract void Update();

    /// <summary>
    /// Called if a fatal exception happens and the game is about to crash
    /// </summary>
    /// <param name="exception">The exception that caused the crash</param>
    protected abstract void OnCrash(Exception exception);

    /// <summary>
    /// Called once after stopping the game.<br/>
    /// Clean up used files or stop music here.
    /// </summary>
    protected abstract void CleanUp();

    private void GameLoop()
    {
        double newTime;
        var accumulator = 0d;
        var frameCount = 0;

        var currentTime = GetHighResolutionTimestamp();

        while (IsRunning)
        {
            newTime = GetHighResolutionTimestamp();
            DeltaTime = newTime - currentTime;
            currentTime = newTime;

            unchecked { TotalFrameCount++; }

            accumulator += DeltaTime;
            frameCount++;

            if (accumulator >= 1)
            {
                FramesPerSecond = frameCount;

                accumulator = 0d;
                frameCount = 0;
            }

            Update();

            if (IsKeyPressed(Settings.ForceStopKey)) _cts.Cancel();
        }
    }

    private void OnSettingsUpdated(object? sender, string propertyName)
    {
        switch (propertyName)
        {
            case nameof(NexusConsoleGameSettings.Title): _console.UpdateTitle(Settings.Title); break;
            case nameof(NexusConsoleGameSettings.ColorPalette): _console.UpdateColorPalette(Settings.ColorPalette); break;
            case nameof(NexusConsoleGameSettings.Font): _console.UpdateFont(Settings.Font); break;
            case nameof(NexusConsoleGameSettings.ForceStopKey): _console.StopGameKey = Settings.ForceStopKey; break;
        }
    }

    private static double GetHighResolutionTimestamp()
    {
        Native.QueryPerformanceCounter(out long timestamp);
        Native.QueryPerformanceFrequency(out long frequency);

        return (double)timestamp / frequency;
    }

    private static bool IsKeyPressed(in NexusKey key) => (Native.GetAsyncKeyState((int)key) & 0x8000) != 0;
}