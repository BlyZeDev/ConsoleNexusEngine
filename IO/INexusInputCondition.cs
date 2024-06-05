namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents an interface for an input condition
/// </summary>
public interface INexusInputCondition
{
    /// <summary>
    /// <see langword="true"/> if the condition is met, otherwise <see langword="false"/>
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns><see cref="bool"/></returns>
    public bool Check(in NexusKey key);

    /// <summary>
    /// <see langword="true"/> if the condition is met, otherwise <see langword="false"/>
    /// </summary>
    /// <param name="mousePosition">The mouse position to check</param>
    /// <returns><see cref="bool"/></returns>
    public bool Check(in NexusCoord mousePosition);
}