namespace ConsoleNexusEngine.Common;

/// <summary>
/// Configuration for your Console game
/// </summary>
public sealed class ConsoleGameConfig
{
    /// <summary>
    /// The title the console should have
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// The font the Console should use
    /// </summary>
    public required NexusFont Font { get; set; }

    /// <summary>
    /// The Framerate the Console game tries to run at
    /// </summary>
    public required Framerate TargetFramerate { get; set; }

    /// <summary>
    /// The Color Palette of the Console
    /// </summary>
    public required ColorPalette ColorPalette { get; set; }

    /// <summary>
    /// The key that stops the game if pressed
    /// </summary>
    public required NexusKey StopGameKey { get; set; }
}