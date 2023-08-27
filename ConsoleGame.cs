namespace ConsoleNexusEngine;

/// <summary>
/// Provides methods for your Console game
/// </summary>
public abstract class ConsoleGame
{
    /// <summary>
    /// Called once before the start of the game. Import game files and resources here.
    /// </summary>
    public abstract void Load();

    /// <summary>
    /// Called before every frame. Do math and other logic here.
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Called after every frame. Render your graphics here.
    /// </summary>
    public abstract void Render();
}