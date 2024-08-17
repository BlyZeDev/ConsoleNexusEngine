namespace ConsoleNexusEngine;

using System.Threading;

/// <summary>
/// The settings for <see cref="ConsoleGame"/>
/// </summary>
public sealed record ConsoleGameSettings
{
    internal static ConsoleGameSettings Default => new();

    private string title;
    private NexusFont font;
    private NexusColorPalette colorPalette;
    private NexusKey stopGameKey;
    private ThreadPriority priority;
    private NexusInputType inputTypes;
    private bool enableMonitoring;

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

    /// <summary>
    /// <see langword="true"/> if monitoring should be enabled, otherwise <see langword="false"/>
    /// </summary>
    public bool EnableMonitoring
    {
        get => enableMonitoring;
        set
        {
            enableMonitoring = value;
            Update(nameof(EnableMonitoring));
        }
    }

    internal event EventHandler<string>? Updated;

    private ConsoleGameSettings()
    {
        title = "ConsoleGame";
        font = new TerminalFont(new NexusSize(10));
        colorPalette = NexusColorPalette.Default;
        stopGameKey = NexusKey.Escape;
        priority = ThreadPriority.Normal;
        inputTypes = NexusInputType.All;
        enableMonitoring = false;
    }

    private void Update(string propertyName) => Updated?.Invoke(this, propertyName);
}