﻿namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using System;
using System.Threading;

/// <summary>
/// Provides methods for your Console game
/// </summary>
public abstract class ConsoleGame
{
    private readonly Thread _game;
    private float uncorrectedSleepDuration => 1000f / TargetFramerate;

    /// <summary>
    /// <see langword="true"/> if the game is running, otherwise <see langword="false"/>
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// The Frames per second the game tries to run at
    /// </summary>
    public Framerate TargetFramerate { get; set; }

    protected ConsoleGame(int windowHeight, int windowWidth, Framerate targetFramerate)
    {
        _game = new Thread(GameLoop) { Priority = ThreadPriority.Highest };

        TargetFramerate = targetFramerate;

        IsRunning = false;
    }

    private void GameLoop()
    {
        DateTime lastTime;

        while (IsRunning)
        {
            if (TargetFramerate.IsUnlimited)
            {
                Update();
                Render();
                continue;
            }

            lastTime = DateTime.UtcNow;

            Update();
            Render();

            var sleepDuration = (int)(uncorrectedSleepDuration - (DateTime.UtcNow - lastTime).TotalMilliseconds);
            if (sleepDuration > 0) Thread.Sleep(sleepDuration);
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