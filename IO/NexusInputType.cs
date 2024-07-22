namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents allowed input types
/// </summary>
[Flags]
public enum NexusInputType
{
    /// <summary>
    /// No input is allowed
    /// </summary>
    None = 0,
    /// <summary>
    /// Only mouse input is allowed
    /// </summary>
    Mouse = 2,
    /// <summary>
    /// Only keyboard input is allowed
    /// </summary>
    Keyboard = 4,
    /// <summary>
    /// Only gamepad is allowed
    /// </summary>
    Gamepad = 8,
    /// <summary>
    /// All inputs are allowed
    /// </summary>
    All = Mouse | Keyboard | Gamepad
}