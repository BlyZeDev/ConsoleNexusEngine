namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents a simple sprite
/// </summary>
public readonly struct NexusSimpleSprite : INexusSprite
{
    /// <inheritdoc/>
    public NexusSpriteMap Map { get; }

    /// <summary>
    /// Initializes a new <see cref="NexusSimpleSprite"/> from a <see cref="NexusSpriteMap"/>
    /// </summary>
    /// <param name="map">The map to create the sprite from</param>
    public NexusSimpleSprite(in NexusSpriteMap map) => Map = map;
}