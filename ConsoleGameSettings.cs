namespace ConsoleNexusEngine;

using System.Threading;

/// <summary>
/// Initial configuration for <see cref="ConsoleGame"/>
/// </summary>
public sealed record ConsoleGameSettings
{
    internal static ConsoleGameSettings Default => new();

    private string title;
    private NexusFont font;
    private Framerate targetFramerate;
    private ColorPalette colorPalette;
    private NexusKey stopGameKey;
    private ThreadPriority priority;
    private bool allowInputs;

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
    /// The Framerate the Console game tries to run at
    /// </summary>
    public Framerate TargetFramerate
    {
        get => targetFramerate;
        set
        {
            targetFramerate = value;
            Update(nameof(TargetFramerate));
        }
    }

    /// <summary>
    /// The Color Palette of the Console
    /// </summary>
    public ColorPalette ColorPalette
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
    /// The thread priority of the game
    /// </summary>
    public ThreadPriority Priority
    {
        get => priority;
        set
        {
            priority = value;
            Update(nameof(Priority));
        }
    }

    /// <summary>
    /// If <see langword="false"/> all inputs are ignored
    /// </summary>
    public bool AllowInputs
    {
        get => allowInputs;
        set
        {
            allowInputs = value;
            Update(nameof(AllowInputs));
        }
    }

    internal event EventHandler<string>? Updated;

    private ConsoleGameSettings()
    {
        title = "ConsoleGame";
        font = new Consolas(10, 10);
        targetFramerate = 60;
        colorPalette = ColorPalette.Default;
        stopGameKey = NexusKey.Escape;
        priority = ThreadPriority.Normal;
        allowInputs = true;
    }

    private void Update(string propertyName) => Updated?.Invoke(this, propertyName);
}