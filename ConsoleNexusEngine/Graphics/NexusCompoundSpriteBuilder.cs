namespace ConsoleNexusEngine.Graphics;

using System.Buffers;
using System.Linq;

/// <summary>
/// Represents a builder to merge sprites without completely overriding the bottom layers
/// </summary>
public sealed class NexusCompoundSpriteBuilder
{
    private readonly SearchValues<char> _nonOverrideChars;
    private readonly SortedDictionary<int, NexusSpriteMap> _spriteMaps;

    private NexusSize largestSprite;

    /// <summary>
    /// Initializes an empty <see cref="NexusCompoundSpriteBuilder"/>
    /// </summary>
    /// <param name="nonOverrideChars">Characters that don't override the underlying characters</param>
    public NexusCompoundSpriteBuilder(params ReadOnlySpan<char> nonOverrideChars)
    {
        Span<char> searchValues = StackAlloc.Allow<char>(nonOverrideChars.Length + 1)
            ? stackalloc char[nonOverrideChars.Length + 1] : new char[nonOverrideChars.Length + 1];
        nonOverrideChars.CopyTo(searchValues);
        searchValues[^1] = char.MinValue;

        _nonOverrideChars = SearchValues.Create(searchValues);
        _spriteMaps = [];

        largestSprite = NexusSize.MinValue;
    }

    /// <summary>
    /// Initializes a <see cref="NexusCompoundSpriteBuilder"/> from a sprite
    /// </summary>
    /// <param name="sprite">The sprite to use as base</param>
    /// <param name="layer">The layer the sprite map should be added</param>
    /// <param name="nonOverrideChars">Characters that don't override the underlying characters</param>
    public NexusCompoundSpriteBuilder(INexusSprite sprite, int layer, params ReadOnlySpan<char> nonOverrideChars) : this(nonOverrideChars)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(layer, nameof(layer));

        _spriteMaps.Add(layer, sprite.Map);

        largestSprite = sprite.Map.Size;
    }

    /// <summary>
    /// Get the <see cref="NexusSpriteMap"/> from the specified <paramref name="layer"/>
    /// </summary>
    /// <param name="layer">The layer of the sprite</param>
    /// <exception cref="KeyNotFoundException"/>
    /// <returns><see cref="NexusSpriteMap"/></returns>
    public NexusSpriteMap GetLayer(int layer) => _spriteMaps[layer];

    /// <summary>
    /// Get the <see cref="NexusSpriteMap"/> from the specified <paramref name="layer"/>
    /// </summary>
    /// <param name="layer">The layer of the sprite</param>
    /// <param name="spriteMap">The sprite map at that layer</param>
    /// <returns><see cref="bool"/></returns>
    public bool TryGetLayer(int layer, out NexusSpriteMap spriteMap) => _spriteMaps.TryGetValue(layer, out spriteMap);

    /// <summary>
    /// Clears all sprite layers and resets the builder
    /// </summary>
    public NexusCompoundSpriteBuilder Clear()
    {
        _spriteMaps.Clear();
        largestSprite = NexusSize.MinValue;

        return this;
    }

    /// <summary>
    /// Remove the sprite at the specific <paramref name="layer"/>
    /// </summary>
    /// <remarks>Does nothing if the layer does not exist</remarks>
    /// <param name="layer">The layer of the sprite to remove</param>
    /// <param name="spriteMap">The sprite map at that layer</param>
    /// <returns></returns>
    public NexusCompoundSpriteBuilder RemoveLayer(int layer, out NexusSpriteMap spriteMap)
    {
        _spriteMaps.Remove(layer, out spriteMap);
        if (largestSprite == spriteMap.Size) largestSprite = _spriteMaps.MaxBy(x => x.Value.Size).Value.Size;

        return this;
    }

    /// <summary>
    /// Adds a pixel as sprite to the current map
    /// </summary>
    /// <param name="coordinate">The coordinate of the pixel</param>
    /// <param name="character">The character of the pixel</param>
    /// <param name="layer">The layer the sprite map should be added. Adds to the top, if <paramref name="layer"/> is -1.<br/>If the layer is already present it will be overriden</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddPixel(in NexusCoord coordinate, in NexusChar character, int layer = -1)
    {
        var mapSize = new NexusSize(1);
        Span<CHARINFO> map = StackAlloc.Allow<CHARINFO>(mapSize.Dimensions) ? stackalloc CHARINFO[mapSize.Dimensions] : new CHARINFO[mapSize.Dimensions];

        map[0] = NativeConverter.ToCharInfo(character);
        return AddSpriteMap(coordinate, new NexusSpriteMap(map, mapSize), layer);
    }

    /// <summary>
    /// Adds multiple pixels as sprite to the current map
    /// </summary>
    /// <param name="character">The character of the pixels</param>
    /// <param name="layer">The layer the sprite map should be added. Adds to the top, if <paramref name="layer"/> is -1.<br/>If the layer is already present it will be overriden</param>
    /// <param name="coordinates">The coordinates of the pixels</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddPixels(in NexusChar character, int layer = -1, params ReadOnlySpan<NexusCoord> coordinates)
    {
        if (coordinates.IsEmpty) return this;
        if (coordinates.Length == 1) return AddPixel(coordinates[0], character, layer);

        var mapSize = NexusSize.MinValue;
        foreach (var coordinate in coordinates)
        {
            mapSize = new NexusSize(Math.Max(mapSize.Width, coordinate.X), Math.Max(mapSize.Height, coordinate.Y), false);
        }
        mapSize += new NexusSize(1);

        Span<CHARINFO> map = StackAlloc.Allow<CHARINFO>(mapSize.Dimensions) ? stackalloc CHARINFO[mapSize.Dimensions] : new CHARINFO[mapSize.Dimensions];

        var nativeChar = NativeConverter.ToCharInfo(character);
        foreach (var coordinate in coordinates)
        {
            map[IndexDimensions.Get1D(coordinate.X, coordinate.Y, mapSize.Width)] = nativeChar;
        }

        return AddSpriteMap(new NexusSpriteMap(map, mapSize), layer);
    }

    /// <summary>
    /// Adds a line as sprite to the current map
    /// </summary>
    /// <param name="start">The start coordinate of the line</param>
    /// <param name="end">The end coordinate of the line</param>
    /// <param name="character">The character of the line</param>
    /// <param name="layer">The layer the sprite map should be added. Adds to the top, if <paramref name="layer"/> is -1.<br/>If the layer is already present it will be overriden</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddLine(in NexusCoord start, in NexusCoord end, in NexusChar character, int layer = -1)
    {
        var nativeChar = NativeConverter.ToCharInfo(character);
        var mapSize = new NexusSize(end.X - start.X + 1, end.Y - start.Y + 1);
        Span<CHARINFO> map = StackAlloc.Allow<CHARINFO>(mapSize.Dimensions) ? stackalloc CHARINFO[mapSize.Dimensions] : new CHARINFO[mapSize.Dimensions];

        var startX = 0;
        var startY = 0;
        var endX = end.X - start.X;
        var endY = end.Y - start.Y;

        if (startX == endX)
        {
            var startCoord = Math.Min(startY, endY);
            var endCoord = Math.Max(startY, endY);

            for (int y = startCoord; y <= endCoord; y++)
            {
                map[IndexDimensions.Get1D(startX, y, mapSize.Width)] = nativeChar;
            }
        }
        else if (startY == endY)
        {
            var startCoord = Math.Min(startX, endX);
            var endCoord = Math.Max(startX, endX);

            for (int x = startCoord; x <= endCoord; x++)
            {
                map[IndexDimensions.Get1D(x, startY, mapSize.Width)] = nativeChar;
            }
        }
        else
        {
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;

            if (endX < 0) dx1 = -1;
            else if (endX > 0) dx1 = 1;

            if (endY < 0) dy1 = -1;
            else if (endY > 0) dy1 = 1;

            if (endX < 0) dx2 = -1;
            else if (endX > 0) dx2 = 1;

            var longest = Math.Abs(endX);
            var shortest = Math.Abs(endY);

            if (!(longest > shortest))
            {
                longest = Math.Abs(endY);
                shortest = Math.Abs(endX);

                if (endY < 0) dy2 = -1;
                else if (endY > 0) dy2 = 1;

                dx2 = 0;
            }

            var numerator = longest >> 1;

            for (int i = 0; i <= longest; i++)
            {
                map[IndexDimensions.Get1D(startX, startY, mapSize.Width)] = nativeChar;

                numerator += shortest;

                if (!(numerator < longest))
                {
                    numerator -= longest;
                    startX += dx1;
                    startY += dy1;
                }
                else
                {
                    startX += dx2;
                    startY += dy2;
                }
            }
        }

        return AddSpriteMap(start, new NexusSpriteMap(map, mapSize), layer);
    }

    /// <summary>
    /// Adds a sprite to the current map
    /// </summary>
    /// <param name="sprite">The sprite to add</param>
    /// <param name="layer">The layer the sprite map should be added. Adds to the top, if <paramref name="layer"/> is -1.<br/>If the layer is already present it will be overriden</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddSprite(INexusSprite sprite, int layer = -1) => AddSpriteMap(sprite.Map, layer);

    /// <summary>
    /// Adds a sprite to the current map at a specific position
    /// </summary>
    /// <param name="coordinate">The top-left coordinate to start from</param>
    /// <param name="sprite">The sprite to add</param>
    /// <param name="layer">The layer the sprite map should be added. Adds to the top, if <paramref name="layer"/> is -1.<br/>If the layer is already present it will be overriden</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddSprite(in NexusCoord coordinate, INexusSprite sprite, int layer = -1) => AddSpriteMap(coordinate, sprite.Map, layer);

    /// <summary>
    /// Adds a sprite to the current map
    /// </summary>
    /// <param name="spriteMap">The sprite map to add</param>
    /// <param name="layer">The layer the sprite map should be added. Adds to the top, if <paramref name="layer"/> is -1.<br/>If the layer is already present it will be overriden</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddSpriteMap(in NexusSpriteMap spriteMap, int layer = -1) => AddSpriteMap(NexusCoord.MinValue, spriteMap, layer);

    /// <summary>
    /// Adds a sprite to the current map at a specific position
    /// </summary>
    /// <param name="coordinate">The top-left coordinate to start from</param>
    /// <param name="spriteMap">The sprite map to add</param>
    /// <param name="layer">The layer the sprite map should be added. Adds to the top, if <paramref name="layer"/> is -1.<br/>If the layer is already present it will be overriden</param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddSpriteMap(in NexusCoord coordinate, in NexusSpriteMap spriteMap, int layer = -1)
    {
        var layeredMap = spriteMap;
        if (coordinate != NexusCoord.MinValue)
        {
            var adjustedSize = new NexusSize(coordinate.X + spriteMap.Size.Width, coordinate.Y + spriteMap.Size.Height);
            var adjustedMap = StackAlloc.Allow<CHARINFO>(adjustedSize.Dimensions)
                ? stackalloc CHARINFO[adjustedSize.Dimensions] : new CHARINFO[adjustedSize.Dimensions];

            var layeredMapSpan = layeredMap._spriteMap.Span;
            for (int y = 0; y < layeredMap.Size.Height; y++)
            {
                layeredMapSpan.Slice(y * layeredMap.Size.Width, layeredMap.Size.Width)
                    .CopyTo(adjustedMap.Slice((coordinate.Y + y) * adjustedSize.Width + coordinate.X, layeredMap.Size.Width));
            }

            layeredMap = new NexusSpriteMap(adjustedMap, adjustedSize);
        }

        largestSprite = new NexusSize(Math.Max(largestSprite.Width, layeredMap.Size.Width), Math.Max(largestSprite.Height, layeredMap.Size.Height));

        layer = layer == -1 ? _spriteMaps.Last().Key + 1 : layer;
        _spriteMaps.Remove(layer);
        _spriteMaps.Add(layer, layeredMap);

        return this;
    }

    /// <summary>
    /// Finalizes the sprite and returns it
    /// </summary>
    /// <returns><see cref="NexusSimpleSprite"/></returns>
    public NexusSimpleSprite Build()
    {
        if (_spriteMaps.Count == 0) return new NexusSimpleSprite(new NexusSpriteMap(NexusSize.MinValue));
        if (_spriteMaps.Count == 1) return new NexusSimpleSprite(_spriteMaps.Values.First());

        Span<CHARINFO> finishedMap = StackAlloc.Allow<CHARINFO>(largestSprite.Dimensions)
            ? stackalloc CHARINFO[largestSprite.Dimensions] : new CHARINFO[largestSprite.Dimensions];

        NexusSpriteMapPixel currentPixel;
        foreach (var map in _spriteMaps.Values)
        {
            for (int y = 0; y < map.Size.Height; y++)
            {
                for (int x = 0; x < map.Size.Width; x++)
                {
                    currentPixel = map[x, y];
                    if (!_nonOverrideChars.Contains(currentPixel.Character))
                    {
                        finishedMap[IndexDimensions.Get1D(x, y, largestSprite.Width)] = NativeConverter.ToCharInfo(currentPixel);
                    }
                }
            }
        }

        return new NexusSimpleSprite(new NexusSpriteMap(finishedMap, largestSprite));
    }
}