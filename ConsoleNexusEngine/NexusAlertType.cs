namespace ConsoleNexusEngine;

/// <summary>
/// Represents an alert type
/// </summary>
public enum NexusAlertType : uint
{
    /// <summary>
    /// The message box contains one push button: OK
    /// </summary>
    Ok = 0x00000000,
    /// <summary>
    /// The message box contains two push buttons: OK and Cancel
    /// </summary>
    OkCancel = 0x00000001,
    /// <summary>
    /// The message box contains three push buttons: Abort, Retry, and Ignore
    /// </summary>
    AbortRetryIgnore = 0x00000002,
    /// <summary>
    /// The message box contains three push buttons: Yes, No, and Cancel
    /// </summary>
    YesNoCancel = 0x00000003,
    /// <summary>
    /// The message box contains two push buttons: Yes and No
    /// </summary>
    YesNo = 0x00000004,
    /// <summary>
    /// The message box contains two push buttons: Retry and Cancel
    /// </summary>
    RetryCancel = 0x00000005,
    /// <summary>
    /// The message box contains three push buttons: Cancel, Try Again, Continue
    /// </summary>
    CancelTryContinue = 0x00000006
}