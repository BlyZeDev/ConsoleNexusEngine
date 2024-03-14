namespace ConsoleNexusEngine.IO;

using System.Linq;

/// <summary>
/// Represents a key press condition
/// </summary>
public sealed class NexusKeyPressCondition : INexusInputCondition
{
    private readonly Predicate<INexusInput> _predicate;

    /// <summary>
    /// Check for a key press
    /// </summary>
    /// <param name="key">The key to check for</param>
    public NexusKeyPressCondition(NexusKey key) => _predicate = (input) => input.Key == key;

    /// <summary>
    /// Check for multiple key presses
    /// </summary>
    /// <param name="keys">The keys to check for</param>
    public NexusKeyPressCondition(params NexusKey[] keys) => _predicate = (input) => keys.Contains(input.Key);

    ///<inheritdoc/>
    public bool Check(INexusInput input) => _predicate(input);
}
