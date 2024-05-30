namespace ConsoleNexusEngine;

/// <summary>
/// Represents an object that can be loaded
/// </summary>
public interface INexusLoadable
{
    /// <summary>
    /// This should be called inside <see cref="ConsoleGame.Load"/>
    /// </summary>
    public void Load();
}