namespace ConsoleNexusEngine;

/// <summary>
/// Represents an object that can be updated every frame
/// </summary>
public interface INexusUpdatable
{
    /// <summary>
    /// This should be called inside <see cref="ConsoleGame.Update(in NexusInputCollection, in double)"/>
    /// </summary>
    /// <param name="inputs">The inputs made during the last frame</param>
    /// <param name="deltaTime">The time elapsed since the last frame in total seconds</param>
    public void Update(in NexusInputCollection inputs, in double deltaTime);
}