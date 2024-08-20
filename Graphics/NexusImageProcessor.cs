namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents an abstract class for image processing
/// </summary>
public abstract class NexusImageProcessor
{
    /// <summary>
    /// The color palette used
    /// </summary>
    protected readonly NexusColorPalette _colorPalette;

    /// <summary>
    /// Initializes an Image Processor
    /// </summary>
    /// <param name="colorPalette">The color palette to use</param>
    protected NexusImageProcessor(NexusColorPalette colorPalette) => _colorPalette = colorPalette;

    /// <summary>
    /// The process to calculate the color index
    /// </summary>
    /// <param name="targetColor">The color to process</param>
    /// <returns><see cref="NexusColor"/></returns>
    public abstract NexusColorIndex Process(in NexusColor targetColor);
}