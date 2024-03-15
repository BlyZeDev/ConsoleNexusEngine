namespace ConsoleNexusEngine.IO;

/// <summary>
/// A generic interface for a console input
/// </summary>
public interface INexusInput
{
    /// <summary>
    /// The key that got pressed
    /// </summary>
    public NexusKey Key { get; }

    /// <summary>
    /// The position of the mouse cursor, or <see langword="null"/> if no mouse position is set
    /// </summary>
    public Coord? MousePosition { get; }
}