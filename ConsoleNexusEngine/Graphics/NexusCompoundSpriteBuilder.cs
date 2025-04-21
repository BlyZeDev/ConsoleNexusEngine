namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents a builder to merge sprites without completely overriding the bottom layers
/// </summary>
public sealed class NexusCompoundSpriteBuilder
{
    private NexusSpriteMap _currentMap;

    private NexusCompoundSpriteBuilder(in NexusSpriteMap map) => _currentMap = map;

    /// <summary>
    /// Adds a sprite on top of the current map
    /// </summary>
    /// <param name="sprite">The sprite to add</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddSprite(INexusSprite sprite) => AddSpriteMap(sprite.Map);

    /// <summary>
    /// Adds a sprite on top of the current map at a specific position
    /// </summary>
    /// <param name="coordinate">The top-left coordinate to start from</param>
    /// <param name="sprite">The sprite to add</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddSprite(in NexusCoord coordinate, INexusSprite sprite) => AddSpriteMap(coordinate, sprite.Map);

    /// <summary>
    /// Adds a sprite map on top of the current map
    /// </summary>
    /// <param name="spriteMap">The sprite map to add</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddSpriteMap(in NexusSpriteMap spriteMap)
    {
        var size = _currentMap.Size.Dimensions > spriteMap.Size.Dimensions ? _currentMap.Size : spriteMap.Size;

        Span<CHARINFO> newMap = stackalloc CHARINFO[size.Dimensions];
        _currentMap._spriteMap.Span.CopyTo(newMap);

        var toWrite = spriteMap._spriteMap.Span;

        var dimensions = spriteMap.Size.Dimensions;
        for (int i = 0; i < dimensions; i++)
        {
            var current = toWrite[i];
            newMap[i] = current;
        }

        _currentMap = new NexusSpriteMap(newMap, size);

        return this;
    }

    /// <summary>
    /// Adds a sprite map on top of the current map at a specific position
    /// </summary>
    /// <param name="coordinate">The top-left coordinate to start from</param>
    /// <param name="spriteMap">The sprite map to add</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddSpriteMap(in NexusCoord coordinate, in NexusSpriteMap spriteMap)
    {
        var size = _currentMap.Size.Dimensions > spriteMap.Size.Dimensions ? _currentMap.Size : spriteMap.Size;

        Span<CHARINFO> newMap = stackalloc CHARINFO[size.Dimensions];
        _currentMap._spriteMap.Span.CopyTo(newMap);

        var toWrite = spriteMap._spriteMap.Span;

        for (int x = coordinate.X; x < coordinate.X + spriteMap.Size.Width - 1; x++)
        {
            for (int y = coordinate.Y; y < coordinate.Y + spriteMap.Size.Height - 1; y++)
            {
                var current = toWrite[IndexDimensions.Get1D(x, y, spriteMap.Size.Width)];
                newMap[IndexDimensions.Get1D(x, y, spriteMap.Size.Width)] = current;
            }
        }

        _currentMap = new NexusSpriteMap(newMap, size);

        return this;
    }

    /// <summary>
    /// Builds the sprite
    /// </summary>
    /// <returns><see cref="NexusCompoundSprite"/></returns>
    public NexusCompoundSprite Build() => new NexusCompoundSprite(_currentMap);

    /// <summary>
    /// Creates a new <see cref="NexusCompoundSpriteBuilder"/>
    /// </summary>
    /// <param name="sprite">The base sprite to use</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public static NexusCompoundSpriteBuilder Create(INexusSprite sprite)
        => new NexusCompoundSpriteBuilder(sprite.Map);
}