namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// A writeable 2-dimensional map of pixels that represent a sprite
/// </summary>
public readonly ref struct NexusWritableSpriteMap
{
    internal readonly Span<CHARINFO> _spriteMap;

    /// <summary>
    /// The width of the sprite map
    /// </summary>
    public readonly int Width { get; }

    /// <summary>
    /// The height of the sprite map
    /// </summary>
    public readonly int Height { get; }

    /// <summary>
    /// Initializes an empty <see cref="NexusSpriteMap"/> with the specified dimensions
    /// </summary>
    /// <param name="width">The width of the sprite map</param>
    /// <param name="height">The height of the sprite map</param>
    public NexusWritableSpriteMap(int width, int height) : this(new CHARINFO[width * height], width, height) { }

    private NexusWritableSpriteMap(CHARINFO[] spriteMap, int width, int height)
    {
        _spriteMap = spriteMap;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// The pixel at this coordinate
    /// </summary>
    /// <param name="x">The X-coordinate of the pixel</param>
    /// <param name="y">The Y-coordinate of the pixel</param>
    /// <returns><see cref="NexusSpriteMapPixel"/></returns>
    public readonly NexusSpriteMapPixel this[int x, int y]
    {
        get => NativeConverter.ToSpriteMapPixel(_spriteMap[IndexDimensions.Get1D(x, y, Width)]);
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
    public NexusSpriteMap AsReadOnly() => new NexusSpriteMap(_spriteMap, Width, Height);
}