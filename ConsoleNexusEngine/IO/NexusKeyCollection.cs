namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents information about the keyboard
/// </summary>
public sealed class NexusKeyCollection
{
    internal readonly HashSet<NexusKey> _previousState;
    internal readonly HashSet<NexusKey> _currentState;

    internal NexusKeyCollection()
    {
        _previousState = [];
        _currentState = [];
    }

    /// <summary>
    /// <see langword="true"/> if the key is down, otherwise <see langword="false"/>
    /// </summary>
    /// <param name="key">The key to check for</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsKeyDown(NexusKey key) => _currentState.Contains(key);

    /// <summary>
    /// <see langword="true"/> if the key is down and wasn't down before, otherwise <see langword="false"/>
    /// </summary>
    /// <param name="key">The key to check for</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsKeyJustDown(NexusKey key) => !_previousState.Contains(key) && _currentState.Contains(key);

    /// <summary>
    /// <see langword="true"/> if the key is up, otherwise <see langword="false"/>
    /// </summary>
    /// <param name="key">The key to check for</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsKeyUp(NexusKey key) => !_currentState.Contains(key);

    /// <summary>
    /// <see langword="true"/> if the key is up and wasn't up before, otherwise <see langword="false"/>
    /// </summary>
    /// <param name="key">The key to check for</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsKeyJustUp(NexusKey key) => _previousState.Contains(key) && !_currentState.Contains(key);
}