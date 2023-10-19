namespace ConsoleNexusEngine.Common;

public sealed partial record NexusFont
{
    /// <summary>
    /// The console font 'Consolas'
    /// </summary>
    /// <param name="width">The width the font should have</param>
    /// <param name="height">The height the font should have</param>
    /// <remarks>
    /// Keep in mind that some characters might not work in this font.<br/>
    /// I would recommend to test if the character is included.
    /// </remarks>
    /// <returns><see cref="NexusFont"/></returns>
    public static NexusFont Consolas(int width, int height)
        => new("Consolas", width, height);

    /// <summary>
    /// The console font 'Terminal'
    /// </summary>
    /// <param name="width">The width the font should have</param>
    /// <param name="height">The height the font should have</param>
    /// <remarks>
    /// Keep in mind that some characters might not work in this font.<br/>
    /// I would recommend to test if the character is included.
    /// </remarks>
    /// <returns><see cref="NexusFont"/></returns>
    public static NexusFont Terminal(int width, int height)
        => new("Terminal", width, height);
}