namespace ConsoleNexusEngine;

/// <summary>
/// Represents the result of an alert
/// </summary>
public enum NexusAlertResult
{
    /// <summary>
    /// The OK button was selected
    /// </summary>
    Ok = 1,
    /// <summary>
    /// The Cancel button was selected
    /// </summary>
    Cancel = 2,
    /// <summary>
    /// The Abort button was selected
    /// </summary>
    Abort = 3,
    /// <summary>
    /// The Retry button was selected
    /// </summary>
    Retry = 4,
    /// <summary>
    /// The Ignore button was selected
    /// </summary>
    Ignore = 5,
    /// <summary>
    /// The Yes button was selected
    /// </summary>
    Yes = 6,
    /// <summary>
    /// The No button was selected
    /// </summary>
    No = 7,
    /// <summary>
    /// The Try Again button was selected
    /// </summary>
    TryAgain = 10,
    /// <summary>
    /// The Continue button was selected
    /// </summary>
    Continue = 11
}