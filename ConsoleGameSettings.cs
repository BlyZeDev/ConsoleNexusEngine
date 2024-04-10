namespace ConsoleNexusEngine;

using System.Threading;

/// <summary>
/// Initial configuration for <see cref="ConsoleGame"/>
/// </summary>
public sealed class ConsoleGameSettings
{
    internal static ConsoleGameSettings Default => new();

    private string title;
    private NexusFont font;
    private Framerate targetFramerate;
    private ColorPalette colorPalette;
    private NexusKey stopGameKey;
    private ThreadPriority priority;
    private bool isInputAllowed;

    /// <summary>
    /// The title the console should have
    /// </summary>
    public string Title
    {
        get => title;
        set
        {
            title = value;
            OnSettingsChange();
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
            OnSettingsChange();
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
            OnSettingsChange();
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
            OnSettingsChange();
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
            OnSettingsChange();
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
            OnSettingsChange();
        }
    }

    /// <summary>
    /// If <see langword="false"/> all inputs are ignored
    /// </summary>
    public bool AllowInputs
    {
        get => isInputAllowed;
        set
        {
            isInputAllowed = value;
            OnSettingsChange();
        }
    }

    internal event EventHandler? Changed;

    private ConsoleGameSettings()
    {
        title = "ConsoleGame";
        font = NexusFont.Consolas(10, 10);
        targetFramerate = 60;
        colorPalette = ColorPalette.Windows;
        stopGameKey = NexusKey.Escape;
        priority = ThreadPriority.Normal;
        isInputAllowed = true;
    }

    internal void OnSettingsChange() => Changed?.Invoke(this, EventArgs.Empty);
}