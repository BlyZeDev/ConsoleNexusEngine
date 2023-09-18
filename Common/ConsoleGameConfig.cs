namespace ConsoleNexusEngine.Common;

/// <summary>
/// Configuration for your Console game
/// </summary>
public sealed class ConsoleGameConfig
{
    /// <summary>
    /// The font the Console should use
    /// </summary>
    public required NexusFont Font { get; init; }

    /// <summary>
    /// The Framerate the Console game tries to run at
    /// </summary>
    public required Framerate TargetFramerate { get; init; }

    /// <summary>
    /// The Color Palette of the Console
    /// </summary>
    public required ColorPalette ColorPalette { get; init; }

    /// <summary>
    /// The key that stops the game if pressed
    /// </summary>
    public required NexusKey StopGameKey { get; init; }
}