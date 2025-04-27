namespace ConsoleNexusEngine.Graphics;

using System.Buffers;

/// <summary>
/// Represents a builder to merge sprites without completely overriding the bottom layers
/// </summary>
public sealed class NexusCompoundSpriteBuilder
{
    private readonly SearchValues<char> _nonOverrideChars;

    private CHARINFO[] currentMap;
    private NexusSize currentSize;

    /// <summary>
    /// Initializes a new <see cref="NexusCompoundSpriteBuilder"/> from a sprite
    /// </summary>
    /// <param name="sprite">The sprite to use as base</param>
    /// <param name="nonOverrideChars">Characters that don't override other characters</param>
    public NexusCompoundSpriteBuilder(INexusSprite sprite, params ReadOnlySpan<char> nonOverrideChars)
    {
        _nonOverrideChars = SearchValues.Create(nonOverrideChars);

        currentMap = sprite.Map._spriteMap.ToArray();
        currentSize = sprite.Map.Size;
    }

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
    public NexusCompoundSpriteBuilder AddSpriteMap(in NexusSpriteMap spriteMap) => AddSpriteMap(NexusCoord.MinValue, spriteMap);

    /// <summary>
    /// Adds a sprite map on top of the current map at a specific position
    /// </summary>
    /// <param name="coordinate">The top-left coordinate to start from</param>
    /// <param name="spriteMap">The sprite map to add</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddSpriteMap(in NexusCoord coordinate, in NexusSpriteMap spriteMap)
    {
        if (currentSize.Width < coordinate.X + spriteMap.Size.Width || currentSize.Height < coordinate.Y + spriteMap.Size.Height)
        {
            var prevSize = currentSize;
            currentSize = new NexusSize(Math.Max(currentSize.Width, coordinate.X + spriteMap.Size.Width), Math.Max(currentSize.Height, coordinate.Y + spriteMap.Size.Height));

            Span<CHARINFO> newMap = stackalloc CHARINFO[currentMap.Length];
            currentMap.AsSpan().CopyTo(newMap);

            Array.Resize(ref currentMap, currentSize.Dimensions);
            Array.Clear(currentMap);

            for (int y = 0; y < prevSize.Height; y++)
            {
                newMap.Slice(y * prevSize.Width, prevSize.Width).CopyTo(currentMap.AsSpan(y * currentSize.Width, prevSize.Width));
            }
        }

        NexusSpriteMapPixel currentPixel;
        for (int y = 0; y < spriteMap.Size.Height; y++)
        {
            for (int x = 0; x < spriteMap.Size.Width; x++)
            {
                currentPixel = spriteMap[x, y];
                if (!_nonOverrideChars.Contains(currentPixel.Character))
                {
                    currentMap[IndexDimensions.Get1D(coordinate.X + x, coordinate.Y + y, currentSize.Width)] = NativeConverter.ToCharInfo(currentPixel);
                }
            }
        }

        return this;
    }

    /// <summary>
    /// Builds the sprite
    /// </summary>
    /// <returns><see cref="NexusCompoundSprite"/></returns>
    public NexusCompoundSprite Build() => new NexusCompoundSprite(new NexusSpriteMap(currentMap, currentSize));
}