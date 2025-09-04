namespace ConsoleNexusEngine;

/// <summary>
/// The settings for <see cref="NexusConsoleGame"/>
/// </summary>
public sealed record NexusConsoleGameSettings
{
    internal static NexusConsoleGameSettings Default => new();

    private string title;
    private NexusFont font;
    private NexusColorPalette colorPalette;
    private NexusKey forceStopKey;

    /// <summary>
    /// The title the console should have
    /// </summary>
    public string Title
    {
        get => title;
        set
        {
            title = value;
            Update(nameof(Title));
        }
    }

    /// <summary>
    /// The font the Console should use
    /// </summary>
    public NexusFont Font
    {
        get => font;
        set
        {
            font = value;
            Update(nameof(Font));
        }
    }

    /// <summary>
    /// The Color Palette of the Console
    /// </summary>
    public NexusColorPalette ColorPalette
    {
        get => colorPalette;
        set
        {
            colorPalette = value;
            Update(nameof(ColorPalette));
        }
    }

    /// <summary>
    /// The key that forces the game to stop if pressed
    /// </summary>
    public NexusKey ForceStopKey
    {
        get => forceStopKey;
        set
        {
            forceStopKey = value;
            Update(nameof(ForceStopKey));
        }
    }

    internal event Action<string>? Updated;

    private NexusConsoleGameSettings()
    {
        title = "ConsoleGame";
        font = new NexusFont("Terminal", new NexusSize(10));
        colorPalette = new DefaultColorPalette();
        forceStopKey = NexusKey.Escape;
    }

    private void Update(string propertyName) => Updated?.Invoke(propertyName);
}