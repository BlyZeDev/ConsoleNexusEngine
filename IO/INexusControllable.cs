namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents an interface for controllable objects
/// </summary>
public interface INexusControllable
{
    /// <summary>
    /// This method should be called in <see cref="ConsoleGame.Update(in NexusInputCollection)"/>
    /// </summary>
    /// <param name="inputs">The inputs made during the last frame</param>
    /// <param name="deltaTime">The time elapsed since the last frame in total seconds</param>
    public void Control(in NexusInputCollection inputs, in double deltaTime);
}