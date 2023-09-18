namespace ConsoleNexusEngine.Common;

public sealed partial record NexusFont
{
    /// <summary>
    /// The console font 'Consolas'
    /// </summary>
    /// <param name="width">The width the font should have</param>
    /// <param name="height">The height the font should have</param>
    /// <returns><see cref="NexusFont"/></returns>
    public static NexusFont Consolas(int width, int height)
        => new ("Consolas", width, height);

    /// <summary>
    /// The console font 'Terminal'
    /// </summary>
    /// <param name="width">The width the font should have</param>
    /// <param name="height">The height the font should have</param>
    /// <returns><see cref="NexusFont"/></returns>
    public static NexusFont Terminal(int width, int height)
        => new("Terminal", width, height);
}