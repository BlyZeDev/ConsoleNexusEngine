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
    private NexusKey stopGameKey;
    private NexusInputType inputTypes;

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
    /// The key that stops the game if pressed
    /// </summary>
    public NexusKey StopGameKey
    {
        get => stopGameKey;
        set
        {
            stopGameKey = value;
            Update(nameof(StopGameKey));
        }
    }

    /// <summary>
    /// The allowed inputs types
    /// </summary>
    public NexusInputType InputTypes
    {
        get => inputTypes;
        set
        {
            inputTypes = value;
            Update(nameof(InputTypes));
        }
    }

    internal event EventHandler<string>? Updated;

    private NexusConsoleGameSettings()
    {
        title = "ConsoleGame";
        font = new NexusFont("Terminal", new NexusSize(10));
        colorPalette = new DefaultColorPalette();
        stopGameKey = NexusKey.Escape;
        inputTypes = NexusInputType.All;
    }

    private void Update(string propertyName) => Updated?.Invoke(this, propertyName);
}