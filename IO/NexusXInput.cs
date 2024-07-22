namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents input from a XInput device
/// </summary>
[Flags]
public enum NexusXInput
{
    /// <summary>
    /// Directional Pad Up
    /// </summary>
    DirectionalPadUp = 0x0001,
    /// <summary>
    /// Directional Pad Down
    /// </summary>
    DirectionalPadDown = 0x0002,
    /// <summary>
    /// Directional Pad Left
    /// </summary>
    DirectionalPadLeft = 0x0004,
    /// <summary>
    /// Directional Pad Right
    /// </summary>
    DirectionalPadRight = 0x0008,
    /// <summary>
    /// Start button
    /// </summary>
    Start = 0x0010,
    /// <summary>
    /// Back button
    /// </summary>
    Back = 0x0020,
    /// <summary>
    /// Left thumb button
    /// </summary>
    LeftThumb = 0x0040,
    /// <summary>
    /// Right thumb button
    /// </summary>
    RightThumb = 0x0080,
    /// <summary>
    /// Left shoulder button
    /// </summary>
    LeftShoulder = 0x0100,
    /// <summary>
    /// Right shoulder button
    /// </summary>
    RightShoulder = 0x0200,
    /// <summary>
    /// A button
    /// </summary>
    ButtonA = 0x1000,
    /// <summary>
    /// B button
    /// </summary>
    ButtonB = 0x2000,
    /// <summary>
    /// X button
    /// </summary>
    ButtonX = 0x4000,
    /// <summary>
    /// Y button
    /// </summary>
    ButtonY = 0x8000
}