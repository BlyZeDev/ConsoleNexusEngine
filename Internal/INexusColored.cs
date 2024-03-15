namespace ConsoleNexusEngine.Internal;

internal interface INexusColored
{
    /// <summary>
    /// The foreground color
    /// </summary>
    public NexusColor Foreground { get; }

    /// <summary>
    /// The background color
    /// </summary>
    public NexusColor? Background { get; }
}