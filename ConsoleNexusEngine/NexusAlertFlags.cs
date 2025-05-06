namespace ConsoleNexusEngine;

/// <summary>
/// Represents an alert flag
/// </summary>
/// <remarks>
/// This enum has flags.
/// </remarks>
[Flags]
public enum NexusAlertFlags
{
    /// <summary>
    /// The text is right-justified
    /// </summary>
    Right = 0x00080000,
    /// <summary>
    /// Displays message and caption text using right-to-left reading order on Hebrew and Arabic systems
    /// </summary>
    RightToLeft = 0x00100000,
    /// <summary>
    /// The message box becomes the foreground window
    /// </summary>
    Foreground = 0x00010000,
    /// <summary>
    /// Overlaps a other windows
    /// </summary>
    TopMost = 0x00040000,
}