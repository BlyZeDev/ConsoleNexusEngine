namespace ConsoleNexusEngine;

using System.Threading;

/// <summary>
/// Initial configuration for <see cref="ConsoleGame"/>
/// </summary>
public sealed record ConsoleGameConfig
{
    /// <summary>
    /// Default configuration used for the console
    /// </summary>
    public static ConsoleGameConfig Default => new();

    /// <summary>
    /// The title the console should have
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// The font the Console should use
    /// </summary>
    public NexusFont Font { get; init; }

    /// <summary>
    /// The Framerate the Console game tries to run at
    /// </summary>
    public Framerate TargetFramerate { get; init; }

    /// <summary>
    /// The Color Palette of the Console
    /// </summary>
    public ColorPalette ColorPalette { get; init; }

    /// <summary>
    /// The key that stops the game if pressed
    /// </summary>
    public NexusKey StopGameKey { get; init; }

    /// <summary>
    /// The thread priority of the game
    /// </summary>
    public ThreadPriority Priority { get; init; }

    /// <summary>
    /// Initializes a default configuration
    /// </summary>
    public ConsoleGameConfig()
    {
        Title = "ConsoleGame";
        Font = NexusFont.Consolas(10, 10);
        TargetFramerate = 60;
        ColorPalette = ColorPalette.Windows;
        StopGameKey = NexusKey.Escape;
        Priority = ThreadPriority.Normal;
    }
}