namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// A writeable 2-dimensional map of pixels that represent a sprite
/// </summary>
public readonly ref struct NexusWritableSpriteMap
{
    internal readonly Span<CHARINFO> _spriteMap;

    /// <summary>
    /// The size of the sprite map
    /// </summary>
    public readonly NexusSize Size { get; }

    /// <summary>
    /// Initializes an empty <see cref="NexusSpriteMap"/> with the specified dimensions
    /// </summary>
    /// <param name="size">The size of the sprite map</param>
    public NexusWritableSpriteMap(in NexusSize size) : this(new CHARINFO[size.Dimensions], size) { }

    /// <summary>
    /// Initializes an empty <see cref="NexusSpriteMap"/> from an existing <see cref="NexusSpriteMap"/>
    /// </summary>
    /// <param name="spriteMap">The sprite map to make writable</param>
    public NexusWritableSpriteMap(in NexusSpriteMap spriteMap)
    {
        spriteMap._spriteMap.Span.CopyTo(_spriteMap);
        Size = spriteMap.Size;
    }

    private NexusWritableSpriteMap(CHARINFO[] spriteMap, scoped in NexusSize size)
    {
        _spriteMap = spriteMap;
        Size = size;
    }

    /// <summary>
    /// The pixel at this coordinate
    /// </summary>
    /// <param name="x">The X-coordinate of the pixel</param>
    /// <param name="y">The Y-coordinate of the pixel</param>
    /// <returns><see cref="NexusSpriteMapPixel"/></returns>
    public readonly NexusSpriteMapPixel this[int x, int y]
    {
        get => NativeConverter.ToSpriteMapPixel(_spriteMap[IndexDimensions.Get1D(x, y, Size.Width)]);
        set => NativeConverter.ToCharInfo(value);
    }

    /// <summary>
    /// The pixel at this coordinate
    /// </summary>
    /// <param name="coordinate">The coordinate of the pixel</param>
    /// <returns><see cref="NexusSpriteMapPixel"/></returns>
    public readonly NexusSpriteMapPixel this[in NexusCoord coordinate]
    {
        get => this[coordinate.X, coordinate.Y];
        set => this[coordinate.X, coordinate.Y] = value;
    }

    /// <summary>
    /// Converts <see cref="NexusWritableSpriteMap"/> to the readonly <see cref="NexusSpriteMap"/>
    /// </summary>
    /// <returns><see cref="NexusSpriteMap"/></returns>
    public NexusSpriteMap AsReadOnly() => new NexusSpriteMap(_spriteMap, Size);
}