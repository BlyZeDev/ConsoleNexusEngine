namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using System;
using System.Threading;

/// <summary>
/// Provides methods for your Console game
/// </summary>
public abstract class ConsoleGame
{
    private readonly Thread _game;

    /// <summary>
    /// <see langword="true"/> if the game is running, otherwise <see langword="false"/>
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// The Frames per second the game tries to run at
    /// </summary>
    public Framerate TargetFramerate { get; }

    protected ConsoleGame(Framerate targetFramerate)
    {
        TargetFramerate = targetFramerate;
        IsRunning = false;

        _game = new Thread(TargetFramerate.IsUnlimited ? GameLoopUnlimited : GameLoopCapped)
        {
            Priority = ThreadPriority.Highest
        };
    }

    private void GameLoopCapped() //genauer machen
    {
        double delayBetweenFrames = 1d / TargetFramerate;
        DateTime processingTime;

        while (IsRunning)
        {
            processingTime = DateTime.UtcNow;

            Update();
            Render();

            var delay = (DateTime.UtcNow - processingTime).TotalSeconds;

            if (delay < delayBetweenFrames)
            {
                var millisecondsToWait = (int)(1000 * (delayBetweenFrames - delay));

                if (millisecondsToWait > 0) Thread.Sleep(millisecondsToWait);
            }
        }
    }

    private void GameLoopUnlimited()
    {
        while (IsRunning)
        {
            Update();
            Render();
        }
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
        => IsRunning = false;

    /// <summary>
    /// Called once before the start of the game.<br/>
    /// Import game files and resources here.
    /// </summary>
    public abstract void Load();

    /// <summary>
    /// Called before every frame.<br/>
    /// Do math and other logic here.
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Called after every frame.<br/>
    /// Render your graphics here.
    /// </summary>
    public abstract void Render();
}