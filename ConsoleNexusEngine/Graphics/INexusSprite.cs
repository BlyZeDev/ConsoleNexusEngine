namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Provides a way to create a sprite that can be rendered efficiently
/// </summary>
public interface INexusSprite
{
    internal const string FileExtension = ".nxs";
    internal const string FileExtensionCompressed = ".nxsc";

    /// <summary>
    /// The sprite data that is rendered
    /// </summary>
    public NexusSpriteMap Map { get; }
}