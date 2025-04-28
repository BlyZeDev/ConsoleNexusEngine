namespace ConsoleNexusEngine.Graphics;

using System.Buffers;

/// <summary>
/// Represents a builder to merge sprites without completely overriding the bottom layers
/// </summary>
public sealed class NexusCompoundSpriteBuilder
{
    private readonly SearchValues<char> _nonOverrideChars;
    private readonly LayeredSpriteDictionary _spriteMaps;

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
        _spriteMaps = new LayeredSpriteDictionary();

        largestSprite = NexusSize.MinValue;
    }

    /// <summary>
    /// Initializes a <see cref="NexusCompoundSpriteBuilder"/> from a sprite
    /// </summary>
    /// <param name="sprite">The sprite to use as base</param>
    /// <param name="nonOverrideChars">Characters that don't override the underlying characters</param>
    public NexusCompoundSpriteBuilder(INexusSprite sprite, params ReadOnlySpan<char> nonOverrideChars)
    {
        Span<char> searchValues = StackAlloc.Allow<char>(nonOverrideChars.Length + 1)
            ? stackalloc char[nonOverrideChars.Length + 1] : new char[nonOverrideChars.Length + 1];
        nonOverrideChars.CopyTo(searchValues);
        searchValues[^1] = char.MinValue;

        _nonOverrideChars = SearchValues.Create(searchValues);
        _spriteMaps = new LayeredSpriteDictionary();
        _spriteMaps.Add(0, sprite.Map);

        largestSprite = sprite.Map.Size;
    }

    /// <summary>
    /// Adds a sprite on top of the current map
    /// </summary>
    /// <param name="sprite">The sprite to add</param>
    /// <param name="layer">The layer the sprite map should be added. Adds to the top, if <see langword="null"/></param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddSprite(INexusSprite sprite, int? layer = null) => AddSpriteMap(sprite.Map, layer);

    /// <summary>
    /// Adds a sprite on top of the current map at a specific position
    /// </summary>
    /// <param name="coordinate">The top-left coordinate to start from</param>
    /// <param name="sprite">The sprite to add</param>
    /// <param name="layer">The layer the sprite map should be added. Adds to the top, if <see langword="null"/></param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddSprite(in NexusCoord coordinate, INexusSprite sprite, int? layer = null) => AddSpriteMap(coordinate, sprite.Map, layer);

    /// <summary>
    /// Adds a sprite map on top of the current map
    /// </summary>
    /// <param name="spriteMap">The sprite map to add</param>
    /// <param name="layer">The layer the sprite map should be added. Adds to the top, if <see langword="null"/></param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddSpriteMap(in NexusSpriteMap spriteMap, int? layer = null) => AddSpriteMap(NexusCoord.MinValue, spriteMap, layer);

    /// <summary>
    /// Adds a sprite map on top of the current map at a specific position
    /// </summary>
    /// <param name="coordinate">The top-left coordinate to start from</param>
    /// <param name="spriteMap">The sprite map to add</param>
    /// <param name="layer">The layer the sprite map should be added. Adds to the top, if <see langword="null"/></param>
    /// <returns><see cref="NexusCompoundSpriteBuilder"/></returns>
    public NexusCompoundSpriteBuilder AddSpriteMap(in NexusCoord coordinate, in NexusSpriteMap spriteMap, int? layer = null)
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
                layeredMapSpan.Slice(y * layeredMap.Size.Width, layeredMap.Size.Width).CopyTo(adjustedMap.Slice(y * adjustedSize.Width, layeredMap.Size.Width));
            }

            layeredMap = new NexusSpriteMap(adjustedMap, adjustedSize);
        }

        if (layeredMap.Size.Dimensions > largestSprite.Dimensions) largestSprite = layeredMap.Size;

        _spriteMaps.Add(layer ?? _spriteMaps.HighestLayer + 1, layeredMap);

        return this;

        NexusSpriteMapPixel currentPixel;
        for (int y = 0; y < spriteMap.Size.Height; y++)
        {
            for (int x = 0; x < spriteMap.Size.Width; x++)
            {
                currentPixel = spriteMap[x, y];
                if (!_nonOverrideChars.Contains(currentPixel.Character))
                {
                    currentMap[IndexDimensions.Get1D(coordinate.X + x, coordinate.Y + y, largestSprite.Width)] = NativeConverter.ToCharInfo(currentPixel);
                }
            }
        }
    }

    /// <summary>
    /// Finalizes the sprite and returns it
    /// </summary>
    /// <returns><see cref="NexusSimpleSprite"/></returns>
    public NexusSimpleSprite Build()
    {
        for ()

        return new NexusSimpleSprite(new NexusSpriteMap(currentMap, largestSprite));
    }

    private sealed class LayeredSpriteDictionary
    {
        private readonly Dictionary<int, NexusSpriteMap> _dictionary;

        public int LowestLayer { get; private set; }
        public int HighestLayer { get; private set; }

        public LayeredSpriteDictionary()
        {
            _dictionary = [];
        }
        public LayeredSpriteDictionary(int layer, in NexusSpriteMap map)
        {
            _dictionary = new Dictionary<int, NexusSpriteMap>
            {
                { layer, map }
            };
        }

        public void Add(int layer, in NexusSpriteMap map)
        {
            _dictionary.Add(layer, map);

            if (layer < LowestLayer)
            {
                LowestLayer = layer;
                return;
            }

            if (layer > HighestLayer) HighestLayer = layer;
        }
    }
}