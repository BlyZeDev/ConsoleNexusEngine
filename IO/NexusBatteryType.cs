namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents the battery type of a XInput device
/// </summary>
public enum NexusBatteryType
{
    /// <summary>
    /// The device is not connected
    /// </summary>
    Disconnected = 0x00,
    /// <summary>
    /// The device is wired and has no battery
    /// </summary>
    Wired = 0x01,
    /// <summary>
    /// The device has a Alkaline battery
    /// </summary>
    Alkaline = 0x02,
    /// <summary>
    /// The device has a Nickel Metal Hydride battery
    /// </summary>
    Nimh = 0x03,
    /// <summary>
    /// Cannot determine the devices battery
    /// </summary>
    Unknown = 0xFF
}