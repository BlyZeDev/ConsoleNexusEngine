namespace ConsoleNexusEngine.Common;

/// <summary>
/// Configuration for your Console game
/// </summary>
public sealed class ConsoleGameConfig
{
    /// <summary>
    /// The Font Width of the console window
    /// </summary>
    public required int FontWidth { get; init; }
    /// <summary>
    /// The Font Height of the console window
    /// </summary>
    public required int FontHeight { get; init; }

    /// <summary>
    /// The Framerate the Console game tries to run at
    /// </summary>
    public required Framerate TargetFramerate { get; init; }
}