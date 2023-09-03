namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal;
using System;
using System.Threading;

/// <summary>
/// Provides methods for your Console game
/// </summary>
public abstract partial class ConsoleGame
{
    private readonly Thread _game;
    private readonly ConsoleGameConfig _config;

    /// <summary>
    /// The Core of the Console Game
    /// </summary>
    public ConsoleEngine Engine { get; }

    /// <summary>
    /// <see langword="true"/> if the game is running, otherwise <see langword="false"/>
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// The total amount of rendered frames
    /// </summary>
    public int TotalFrameCount { get; private set; }

    /// <summary>
    /// The Frames per second the game tries to run at
    /// </summary>
    public Framerate TargetFramerate => _config.TargetFramerate;

    /// <summary>
    /// Initializes the <see cref="ConsoleGame"/>
    /// </summary>
    /// <param name="config">The configuration for the console game</param>
    protected ConsoleGame(ConsoleGameConfig config)
    {
        IsRunning = false;

        Engine = new();

        _config = config;

        _game = new Thread(TargetFramerate.IsUnlimited ? GameLoopUnlimited : GameLoopCapped)
        {
            Priority = ThreadPriority.Highest
        };
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    public void Start()
    {
        Load();

        IsRunning = true;

        _game.Start();
    }

    /// <summary>
    /// Stops the game
    /// </summary>
    public void Stop()
    {
        IsRunning = false;

        _game.Join();
    }

    /// <summary>
    /// Called once before the start of the game.<br/>
    /// Import game files and resources here.
    /// </summary>
    public abstract void Load();

    /// <summary>
    /// Called before every frame.<br/>
    /// Do math and other logic here.
    /// </summary>
    public abstract void Update(in ReadOnlySpan<GameInputKey> pressedKeys);

    /// <summary>
    /// Called after every frame.<br/>
    /// Render your graphics here.
    /// </summary>
    public abstract void Render();

    private void GameLoopCapped()
    {
        var targetFrameTime = 1d / TargetFramerate;

        var currentTime = Native.GetHighResolutionTimestamp();
        var accumulator = 0d;

        while (IsRunning)
        {
            var newTime = Native.GetHighResolutionTimestamp();
            var frameTime = newTime - currentTime;
            currentTime = newTime;

            accumulator += frameTime;

            while (accumulator >= targetFrameTime)
            {
                Update(GetPressedKeys());
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
            Update(GetPressedKeys());
            unchecked { TotalFrameCount++; }
            Render();
        }
    }
}