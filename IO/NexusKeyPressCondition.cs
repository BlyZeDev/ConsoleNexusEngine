namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents a key press condition
/// </summary>
public readonly struct NexusKeyPressCondition : INexusInputCondition
{
    private readonly NexusKey _key;

    /// <summary>
    /// Initializes an empty key press check
    /// </summary>
    public NexusKeyPressCondition() : this(NexusKey.None) { }

    /// <summary>
    /// Checks for a key press
    /// </summary>
    /// <param name="key">The key to check for</param>
    public NexusKeyPressCondition(in NexusKey key) => _key = key;

    /// <inheritdoc/>
    public bool Check(in NexusKey key) => _key == key;

    /// <summary>
    /// Returns <see langword="false"/>
    /// </summary>
    /// <param name="mousePosition"><inheritdoc/></param>
    /// <returns><see cref="bool"/></returns>
    public bool Check(in NexusCoord mousePosition) => false;
}