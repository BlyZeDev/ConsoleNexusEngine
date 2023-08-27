namespace ConsoleNexusEngine.Common;

/// <summary>
/// Configuration for your Console game
/// </summary>
public sealed class ConsoleGameConfig
{
    /// <summary>
    /// The width of the Console
    /// </summary>
    public int ConsoleWidth { get; init; }

    /// <summary>
    /// The height of the Console
    /// </summary>
    public int ConsoleHeight { get; init; }

    /// <summary>
    /// The Framerate the Console game tries to run at
    /// </summary>
    public Framerate TargetFramerate { get; init; }
}