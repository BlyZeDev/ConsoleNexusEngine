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

        var offsetX = Math.Min(start.X, end.X);
        var offsetY = Math.Min(start.Y, end.Y);

        var mapSize = new NexusSize(Math.Abs(end.X - start.X) + 1, Math.Abs(end.Y - start.Y) + 1);
        Span<CHARINFO> map = StackAlloc.Allow<CHARINFO>(mapSize.Dimensions) ? stackalloc CHARINFO[mapSize.Dimensions] : new CHARINFO[mapSize.Dimensions];

        var x0 = start.X - offsetX;
        var y0 = start.Y - offsetY;
        var x1 = end.X - offsetX;
        var y1 = end.Y - offsetY;

        var dx = Math.Abs(x1 - x0);
        var sx = x0 < x1 ? 1 : -1;
        var dy = -Math.Abs(y1 - y0);
        var sy = y0 < y1 ? 1 : -1;
        var error = dx + dy;

        while (true)
        {
            var index = IndexDimensions.Get1D(x0, y0, mapSize.Width);
            map[index] = nativeChar;

            if (x0 == x1 && y0 == y1) break;
            var e2 = 2 * error;
            if (e2 >= dy) { error += dy; x0 += sx; }
            if (e2 <= dx) { error += dx; y0 += sy; }
        }

        return AddSpriteMap(new NexusCoord(offsetX, offsetY), new NexusSpriteMap(map, mapSize), layer);
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
    /// Finalizes the sprite and returns it as a <see cref="NexusSpriteMap"/>
    /// </summary>
    /// <returns><see cref="NexusSpriteMap"/></returns>
    public NexusSpriteMap BuildMap()
    {
        if (_spriteMaps.Count == 0) return new NexusSpriteMap(NexusSize.MinValue);
        if (_spriteMaps.Count == 1) return _spriteMaps.Values.First();

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

        return new NexusSpriteMap(finishedMap, largestSprite);
    }

    /// <summary>
    /// Finalizes the sprite and returns it as a <see cref="NexusSimpleSprite"/>
    /// </summary>
    /// <returns><see cref="NexusSimpleSprite"/></returns>
    public NexusSimpleSprite Build() => new NexusSimpleSprite(BuildMap());
}