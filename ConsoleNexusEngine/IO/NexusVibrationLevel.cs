namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents a vibration level
/// </summary>
public enum NexusVibrationLevel
{
    /// <summary>
    /// No vibration
    /// </summary>
    None = 0,
    /// <summary>
    /// The minimum vibration strength
    /// </summary>
    Minimum = 8000,
    /// <summary>
    /// Low vibration strength
    /// </summary>
    Low = 16000,
    /// <summary>
    /// Medium vibration strength
    /// </summary>
    Medium = 32000,
    /// <summary>
    /// High vibration strength
    /// </summary>
    High = 48000,
    /// <summary>
    /// The maximum vibration strength
    /// </summary>
    Maximum = ushort.MaxValue
}