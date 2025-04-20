namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// A 2-dimensional map of pixels that represent a sprite
/// </summary>
public readonly struct NexusSpriteMap
{
    internal readonly ReadOnlyMemory<CHARINFO> _spriteMap;

    /// <summary>
    /// The size of the sprite map
    /// </summary>
    public readonly NexusSize Size { get; }

    /// <summary>
    /// Initializes an empty <see cref="NexusSpriteMap"/> with the specified dimensions
    /// </summary>
    /// <param name="width">The width of the sprite map</param>
    /// <param name="height">The height of the sprite map</param>
    public NexusSpriteMap(int width, int height) : this(new CHARINFO[width * height], width, height) { }

    internal NexusSpriteMap(CHARINFO info) : this([info], 1, 1) { }

    internal NexusSpriteMap(Span<CHARINFO> spriteMap, int width, int height)
    {
        _spriteMap = new ReadOnlyMemory<CHARINFO>(spriteMap.ToArray());
        Size = new NexusSize(width, height);
    }

    /// <summary>
    /// The pixel at this coordinate
    /// </summary>
    /// <param name="x">The X-coordinate of the pixel</param>
    /// <param name="y">The Y-coordinate of the pixel</param>
    /// <returns><see cref="NexusSpriteMapPixel"/></returns>
    public readonly NexusSpriteMapPixel this[int x, int y] => NativeConverter.ToSpriteMapPixel(_spriteMap.Span[IndexDimensions.Get1D(x, y, Size.Width)]);

    /// <summary>
    /// The pixel at this coordinate
    /// </summary>
    /// <param name="coordinate">The coordinate of the pixel</param>
    /// <returns><see cref="NexusSpriteMapPixel"/></returns>
    public readonly NexusSpriteMapPixel this[in NexusCoord coordinate] => this[coordinate.X, coordinate.Y];
}