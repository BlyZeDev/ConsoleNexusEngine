namespace ConsoleNexusEngine;

/// <summary>
/// Represents an object that can be loaded
/// </summary>
public interface INexusUnloadable
{
    /// <summary>
    /// This should be called inside <see cref="ConsoleGame.CleanUp"/>
    /// </summary>
    public void Unload();
}