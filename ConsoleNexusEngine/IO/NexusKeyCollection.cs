namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents information about keyboard and mouse buttons
/// </summary>
public sealed class NexusKeyCollection
{
    private readonly HashSet<NexusKey> _previousState;
    internal readonly HashSet<NexusKey> _currentState;

    /// <summary>
    /// Contains all buttons pressed the update before the most recent update
    /// </summary>
    /// <remarks>
    /// This is just for enumeration
    /// </remarks>
    public IReadOnlySet<NexusKey> PreviousState => _previousState;
    
    /// <summary>
    /// Contains all buttons pressed at the most recent update
    /// </summary>
    /// <remarks>
    /// This is just for enumeration. To check if a specific <see cref="NexusKey"/> is contained use <see cref="IsKeyDown(NexusKey)"/>
    /// </remarks>
    public IReadOnlySet<NexusKey> CurrentState => _currentState;

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

    internal void InvalidateCurrent()
    {
        _previousState.Clear();
        _previousState.UnionWith(_currentState);
    }
}