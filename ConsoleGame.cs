namespace ConsoleNexusEngine;

using BackgroundTimer;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides methods for your Console game
/// </summary>
public abstract class ConsoleGame : IDisposable
{
    /// <summary>
    /// The width of the primary screen in pixeln
    /// </summary>
    public static int ScreenWidth { get; }

    /// <summary>
    /// The height of the primary screen in pixeln
    /// </summary>
    public static int ScreenHeight { get; }

    static ConsoleGame()
    {
        ScreenWidth = Native.GetSystemMetrics(0);
        ScreenHeight = Native.GetSystemMetrics(1);
    }

    private readonly Thread _game;
    private readonly CmdConsole _console;
    private readonly BackgroundTimer _fpsTimer;

    private bool shouldStop;
    private float deltaTime;
    private int lastTotalFrameCount;

    /// <summary>
    /// The configuration of the game
    /// </summary>
    protected ConsoleGameConfig Config { get; }

    /// <summary>
    /// Renders all graphics
    /// </summary>
    protected ConsoleGraphic Graphic { get; }

    /// <summary>
    /// Input handler and controller
    /// </summary>
    protected ConsoleController Controller { get; }

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
    /// The current FPS count
    /// </summary>
    public int FramesPerSecond { get; private set; }

    /// <summary>
    /// The time the game started, set in <see cref="Start()"/>
    /// </summary>
    public DateTime StartTime { get; private set; }

    /// <summary>
    /// Set the speed the frames should be rendered
    /// </summary>
    /// <remarks><see cref="DeltaTime"/> is ignored when <see cref="ConsoleGameConfig.TargetFramerate"/> is <see cref="Framerate.Unlimited"/></remarks>
    public float DeltaTime
    {
        get => deltaTime;
        protected set => deltaTime = Math.Clamp(value, 0, 100);
    }

    /// <summary>
    /// If <see langword="false"/> all inputs are ignored
    /// </summary>
    public bool IsInputAllowed { get; protected set; }

    /// <summary>
    /// The width of the console in characters
    /// </summary>
    public int Width => _console.Width;

    /// <summary>
    /// The height of the console in characters
    /// </summary>
    public int Height => _console.Height;

    /// <summary>
    /// The background color of the whole console
    /// </summary>
    public NexusColor Background => Config.ColorPalette[Graphic.BackgroundIndex];

    /// <summary>
    /// Initializes the <see cref="ConsoleGame"/>
    /// </summary>
    /// <param name="config">The configuration for the console game</param>
    protected ConsoleGame(ConsoleGameConfig config)
    {
        IsRunning = false;

        Config = config;

        _console = new CmdConsole(Config.Title, Config.ColorPalette, Config.Font);

        _game = new Thread(Config.TargetFramerate.IsUnlimited ? GameLoopUnlimited : GameLoopCapped)
        {
            Priority = Config.Priority
        };

        _fpsTimer = new BackgroundTimer();

        Graphic = new(_console);
        Controller = new(_console);
        Utility = new(Config.ColorPalette);

        DeltaTime = 1f;
        IsInputAllowed = true;
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    public void Start()
    {
        Load();

        IsRunning = true;

        StartTime = DateTime.Now;

        _game.Start();

        _fpsTimer.Start(TimeSpan.FromSeconds(1), (ticks) =>
        {
            FramesPerSecond = TotalFrameCount - lastTotalFrameCount;
            lastTotalFrameCount = TotalFrameCount;
            FixedUpdate();
        });
    }

    /// <summary>
    /// Pauses the current thread while the game is running until the <see cref="ConsoleGameConfig.StopGameKey"/> is pressed
    /// </summary>
    public void WaitForStop()
    {
        SpinWait.SpinUntil(() => shouldStop);
    }

    /// <summary>
    /// Stops the game
    /// </summary>
    public void Stop()
    {
        IsRunning = false;

        shouldStop = true;

        _fpsTimer.Stop();

        Unload();

        _game.Join();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (IsRunning) Stop();

        _fpsTimer.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Called once before the start of the game.<br/>
    /// Import game files and resources here.
    /// </summary>
    protected abstract void Load();

    /// <summary>
    /// Called every frame
    /// </summary>
    /// <param name="inputs">The inputs made during the last frame</param>
    protected abstract void Update(in ReadOnlySpan<INexusInput> inputs);

    /// <summary>
    /// Called every second
    /// </summary>
    protected abstract void FixedUpdate();

    /// <summary>
    /// Called once after stopping the game.<br/>
    /// Clean up used files or stop music here.
    /// </summary>
    protected abstract void Unload();

    private void GameLoopCapped()
    {
        var targetFrameTime = 1d / Config.TargetFramerate;
        var stopKey = Config.StopGameKey;

        var currentTime = GetHighResolutionTimestamp();
        var accumulator = 0d;

        double newTime;
        double frameTime;
        double sleepTime;

        while (IsRunning)
        {
            newTime = GetHighResolutionTimestamp();
            frameTime = (newTime - currentTime) * DeltaTime;
            currentTime = newTime;

            accumulator += frameTime;

            while (accumulator >= targetFrameTime)
            {
                unchecked { TotalFrameCount++; }

                if (Controller.IsKeyPressed(stopKey)) shouldStop = true;

                Update(IsInputAllowed ? _console.ReadInput() : []);

                accumulator -= targetFrameTime;
            }

            sleepTime = targetFrameTime - accumulator;

            if (sleepTime > 0) Thread.Sleep((int)(sleepTime * 1000));
        }
    }

    private void GameLoopUnlimited()
    {
        var stopKey = Config.StopGameKey;

        while (IsRunning)
        {
            unchecked { TotalFrameCount++; }

            if (Controller.IsKeyPressed(stopKey)) shouldStop = true;

            Update(IsInputAllowed ? _console.ReadInput() : []);
        }
    }

    private static double GetHighResolutionTimestamp()
    {
        Native.QueryPerformanceCounter(out long timestamp);
        Native.QueryPerformanceFrequency(out long frequency);

        return (double)timestamp / frequency;
    }
}