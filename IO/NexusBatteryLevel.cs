namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents the battery level of a XInput device
/// </summary>
public enum NexusBatteryLevel
{
    /// <summary>
    /// The devices battery is empty
    /// </summary>
    Empty = 0x00,
    /// <summary>
    /// The devices battery is low
    /// </summary>
    Low = 0x01,
    /// <summary>
    /// The devices battery is medium
    /// </summary>
    Medium = 0x02,
    /// <summary>
    /// The devices battery is full
    /// </summary>
    Full = 0x03
}