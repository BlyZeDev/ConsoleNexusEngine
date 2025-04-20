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
    public NexusCompoundSpriteBuilder AddSprite(INexusSprite sprite)
    {
        ResizeIfNeeded(sprite.Map.Size);

        var writableMap = new NexusWritableSpriteMap(_currentMap);
        var toWrite = sprite.Map._spriteMap.Span;

        var dimensions = sprite.Map.Size.Dimensions;
        for (int i = 0; i < dimensions; i++)
        {
            var current = toWrite[i];
            if (current.Attributes != 0) writableMap._spriteMap[i] = current;
        }

        _currentMap = writableMap.AsReadOnly();

        return this;
    }

    private void ResizeIfNeeded(in NexusSize size)
    {
        if (_currentMap.Size.Width >= size.Width && _currentMap.Size.Height >= size.Height) return;

        Span<CHARINFO> newMap = stackalloc CHARINFO[size.Dimensions];
        _currentMap._spriteMap.Span.CopyTo(newMap);

        _currentMap = new NexusSpriteMap(newMap, size);
    }

    /// <summary>
    /// Creates a new <see cref="NexusCompoundSpriteBuilder"/>
    /// </summary>
    /// <param name="sprite">The base sprite to use</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public static NexusCompoundSpriteBuilder Create(INexusSprite sprite)
        => new NexusCompoundSpriteBuilder(sprite.Map);
}