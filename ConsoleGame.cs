namespace ConsoleNexusEngine;

using BackgroundTimer;
using System.Threading;

/// <summary>
/// Provides methods for your Console game
/// </summary>
public abstract class ConsoleGame : IDisposable
{
    private static bool hasInstance;

    /// <summary>
    /// The size of the primary screen in pixels
    /// </summary>
    public static NexusSize ScreenSize { get; }

    static ConsoleGame()
    {
        ScreenSize = new NexusSize(Native.GetSystemMetrics(0), Native.GetSystemMetrics(1));

        hasInstance = false;
    }

    private readonly Thread _game;
    private readonly CmdConsole _console;
    private readonly BackgroundTimer _fpsTimer;

    private int lastTotalFrameCount;

    /// <summary>
    /// The settings of the game
    /// </summary>
    protected ConsoleGameSettings Settings { get; }

    /// <summary>
    /// Renders all graphics
    /// </summary>
    protected ConsoleGraphic Graphic { get; }

    /// <summary>
    /// Useful utility functions
    /// </summary>
    protected ConsoleGameUtil Utility { get; }

    /// <summary>
    /// <see langword="true"/> if the game is running, otherwise <see langword="false"/>
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// The total amount of rendered frames
    /// </summary>
    public int TotalFrameCount { get; private set; }

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
    public NexusColor Background => Settings.ColorPalette[Graphic.BackgroundIndex];

    /// <summary>
    /// Initializes the <see cref="ConsoleGame"/>
    /// </summary>
    protected ConsoleGame()
    {
        if (hasInstance) throw new NexusEngineException("There is already a Console Game instance running.\nPlease dispose it before running a new Console Game");

        hasInstance = true;

        IsRunning = false;

        _console = new CmdConsole(ConsoleGameSettings.Default);

        _game = new Thread(GameLoop);

        _fpsTimer = new BackgroundTimer();

        Settings = ConsoleGameSettings.Default;
        Graphic = new(_console, Settings);
        Utility = new(_console, Settings);

        _game.Priority = Settings.Priority;

        Settings.Updated += OnSettingsUpdated;
    }

    /// <summary>
    /// Starts the game and pauses the current thread while the game is running until the <see cref="ConsoleGameSettings.StopGameKey"/> is pressed
    /// </summary>
    public void Start()
    {
        if (_console.stopGameKeyPressed) throw new NexusEngineException("Can't restart a console game");

        Load();

        IsRunning = true;

        StartTime = DateTimeOffset.Now.DateTime;

        _game.Start();

        _fpsTimer.Start(TimeSpan.FromSeconds(1), (ticks) =>
        {
            FramesPerSecond = TotalFrameCount - lastTotalFrameCount;
            lastTotalFrameCount = TotalFrameCount;
        });

        SpinWait.SpinUntil(() => _console.stopGameKeyPressed);
    }

    /// <summary>
    /// Stops the game
    /// </summary>
    public void Stop()
    {
        IsRunning = false;

        _console.stopGameKeyPressed = true;

        _fpsTimer.Stop();

        CleanUp();

        _game.Join();

        _console.ResetToDefault();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (IsRunning) Stop();

        _fpsTimer.Dispose();

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
    /// <param name="inputs">The inputs made during the last frame</param>
    protected abstract void Update(in NexusInputCollection inputs);

    /// <summary>
    /// Called once after stopping the game.<br/>
    /// Clean up used files or stop music here.
    /// </summary>
    protected abstract void CleanUp();

    private void GameLoop()
    {
        double newTime;

        var currentTime = GetHighResolutionTimestamp();

        while (IsRunning)
        {
            newTime = GetHighResolutionTimestamp();
            DeltaTime = newTime - currentTime;
            currentTime = newTime;

            unchecked { TotalFrameCount++; }

            Update(_console.ReadInput(Settings.StopGameKey, Settings.InputTypes));
        }
    }

    private void OnSettingsUpdated(object? sender, string propertyName)
    {
        switch (propertyName)
        {
            case nameof(ConsoleGameSettings.Title): _console.UpdateTitle(Settings.Title); break;
            case nameof(ConsoleGameSettings.ColorPalette): _console.UpdateColorPalette(Settings.ColorPalette); break;
            case nameof(ConsoleGameSettings.Font): _console.UpdateFont(Settings.Font); break;
        }
    }

    private static double GetHighResolutionTimestamp()
    {
        Native.QueryPerformanceCounter(out long timestamp);
        Native.QueryPerformanceFrequency(out long frequency);

        return (double)timestamp / frequency;
    }
}