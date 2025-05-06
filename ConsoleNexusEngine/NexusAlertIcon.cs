namespace ConsoleNexusEngine;

/// <summary>
/// Represents an alert icon
/// </summary>
public enum NexusAlertIcon : uint
{
    /// <summary>
    /// No icon is used
    /// </summary>
    None = 0x00000000,
    /// <summary>
    /// A stop-sign icon that indicates an error
    /// </summary>
    Error = 0x00000010,
    /// <summary>
    /// A question-mark icon that indicates a question
    /// </summary>
    Question = 0x00000020,
    /// <summary>
    /// An exclamation-point icon that indicates a warning
    /// </summary>
    Warning = 0x00000030,
    /// <summary>
    /// A lowercase letter i in a circle that indicates an information
    /// </summary>
	Information = 0x00000040
}