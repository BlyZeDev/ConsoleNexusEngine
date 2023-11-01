namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides methods for your Console game
/// </summary>
public abstract partial class ConsoleGame
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
        ScreenWidth = Native.GetScreenWidth();
        ScreenHeight = Native.GetScreenHeight();
    }

    private readonly Thread _game;
    private readonly ConsoleGameConfig _config;
    private readonly CancellationTokenSource _cts;

    private float deltaTime;

    /// <summary>
    /// The Core of the Console Game
    /// </summary>
    protected ConsoleEngine Engine { get; }

    /// <summary>
    /// Useful utility for the Console Game
    /// </summary>
    protected ConsoleGameUtil Utility { get; }

    /// <summary>
    /// Plays music and sounds for the Console Game
    /// </summary>
    protected ConsoleGameMusic MusicPlayer { get; }

    /// <summary>
    /// <see langword="true"/> if the game is running, otherwise <see langword="false"/>
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// The total amount of rendered frames
    /// </summary>
    public int TotalFrameCount { get; private set; }

    /// <summary>
    /// The time the game started, set in <see cref="Start()"/>
    /// </summary>
    public DateTime StartTime { get; private set; }

    /// <summary>
    /// Set the speed the frames should be rendered
    /// </summary>
    /// <remarks><see cref="DeltaTime"/> only works when the <see cref="TargetFramerate"/> is not <see cref="Framerate.Unlimited"/></remarks>
    public float DeltaTime
    {
        get => deltaTime;
        protected set => deltaTime = Math.Clamp(value, 0, 100);
    }

    /// <summary>
    /// The Frames per second the game tries to run at
    /// </summary>
    public Framerate TargetFramerate => _config.TargetFramerate;

    /// <summary>
    /// The key that stops the game if pressed
    /// </summary>
    public NexusKey StopGameKey => _config.StopGameKey;

    /// <summary>
    /// The title of the console
    /// </summary>
    public string Title => Engine.Title;

    /// <summary>
    /// The Color Palette of the console
    /// </summary>
    public ColorPalette ColorPalette => Engine.ColorPalette;

    /// <summary>
    /// The width of the console in characters
    /// </summary>
    public int Width => Engine.Width;

    /// <summary>
    /// The height of the console in characters
    /// </summary>
    public int Height => Engine.Height;

    /// <summary>
    /// The font the console uses
    /// </summary>
    public NexusFont Font => Engine.Font;

    /// <summary>
    /// The background color of the whole console
    /// </summary>
    public NexusColor Background => ColorPalette[Engine.Background];

    /// <summary>
    /// Initializes the <see cref="ConsoleGame"/>
    /// </summary>
    /// <param name="config">The configuration for the console game</param>
    protected ConsoleGame(ConsoleGameConfig config)
    {
        IsRunning = false;

        Engine = new(config.Font, config.ColorPalette, config.Title);
        Utility = new();
        MusicPlayer = new();

        _cts = new();

        _config = config;

        _game = new Thread(TargetFramerate.IsUnlimited ? GameLoopUnlimited : GameLoopCapped)
        {
            Priority = ThreadPriority.Highest
        };

        DeltaTime = 1f;
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
    }

    /// <summary>
    /// Pauses the current thread while the game is running until the <see cref="StopGameKey"/> is pressed
    /// </summary>
    public void WaitForStop()
    {
        try
        {
            Task.Delay(-1, _cts.Token).Wait();
        }
        catch (TaskCanceledException) { }
    }

    /// <summary>
    /// Stops the game
    /// </summary>
    public void Stop()
    {
        _cts.Cancel();

        IsRunning = false;

        _game.Join();
    }

    /// <summary>
    /// Called once before the start of the game.<br/>
    /// Import game files and resources here.
    /// </summary>
    protected abstract void Load();

    /// <summary>
    /// Called every frame.<br/>
    /// Do math and other logic here.
    /// </summary>
    /// <param name="inputs">The inputs made during the last frame</param>
    protected abstract void Update(in ReadOnlySpan<INexusInput> inputs);

    /// <summary>
    /// Called after <see cref="Update(in ReadOnlySpan{INexusInput})"/><br/>
    /// Render your graphics here.
    /// </summary>
    protected abstract void Render();

    private void GameLoopCapped()
    {
        var targetFrameTime = 1d / TargetFramerate;

        var currentTime = Native.GetHighResolutionTimestamp();
        double newTime;
        var accumulator = 0d;

        while (IsRunning)
        {
            newTime = Native.GetHighResolutionTimestamp();
            var frameTime = (newTime - currentTime) * deltaTime;
            currentTime = newTime;

            accumulator += frameTime;

            while (accumulator >= targetFrameTime)
            {
                if (Native.IsKeyPressed(StopGameKey)) _cts.Cancel();

                Update(Native.ReadConsoleInput(Engine._console.StandardInput));

                unchecked { TotalFrameCount++; }

                Render();

                accumulator -= targetFrameTime;
            }

            var sleepTime = targetFrameTime - accumulator;

            if (sleepTime > 0)
            {
                int sleepMilliseconds = (int)(sleepTime * 1000);
                Thread.Sleep(sleepMilliseconds);
            }
        }
    }

    private void GameLoopUnlimited()
    {
        while (IsRunning)
        {
            if (Native.IsKeyPressed(StopGameKey)) _cts.Cancel();

            Update(Native.ReadConsoleInput(Engine._console.StandardInput));

            unchecked { TotalFrameCount++; }

            Render();
        }
    }
}