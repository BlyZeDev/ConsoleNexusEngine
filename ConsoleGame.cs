namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
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
    public Framerate TargetFramerate { get; set; }

    protected ConsoleGame(int windowHeight, int windowWidth, Framerate targetFramerate)
    {
        _game = new Thread(GameLoop);

        TargetFramerate = targetFramerate;

        IsRunning = false;
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
    public abstract void Update();

    /// <summary>
    /// Called after every frame.<br/>
    /// Render your graphics here.
    /// </summary>
    public abstract void Render();

    private void GameLoop()
    {

    }

    public void Start()
    {
        IsRunning = true;

        _game.Start();

        while (IsRunning) { }
    }

    public void Stop()
        => IsRunning = false;
}