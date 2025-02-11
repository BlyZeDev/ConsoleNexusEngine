namespace ConsoleNexusEngine.Graphics;

using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
/// Represents an ellipse shape
/// </summary>
public readonly struct NexusEllipse : INexusShape, ISprite
{
    private readonly ReadOnlyMemory2D<CHAR_INFO> _sprite;

    readonly ReadOnlyMemory2D<CHAR_INFO> ISprite.Sprite => _sprite;

    /// <inheritdoc/>
    public readonly NexusSize Size { get; }

    /// <inheritdoc/>
    public readonly NexusChar Character { get; }

    /// <inheritdoc/>
    public readonly bool Fill { get; }

    /// <summary>
    /// Initializes a new <see cref="NexusEllipse"/>
    /// </summary>
    /// <param name="size">The size of the shape</param>
    /// <param name="character">The character to draw</param>
    /// <param name="fill"><see langword="true"/> if the shape is filled, otherwise <see langword="false"/></param>
    public NexusEllipse(in NexusSize size, in NexusChar character, in bool fill)
    {
        var bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format16bppRgb555);

        Size = size;
        Character = character;
        Fill = fill;

        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.DrawEllipse(INexusShape.Red, 0, 0, size.Width - 1, size.Height - 1);
            if (Fill) graphics.FillEllipse(INexusShape.Red.Brush, 0, 0, size.Width - 1, size.Height - 1);
        }

        _sprite = CreateSprite(bitmap, Character);
    }

    /// <summary>
    /// Initializes a new <see cref="NexusEllipse"/>
    /// </summary>
    /// <param name="start">The coordinate of the start of the shape</param>
    /// <param name="end">The coordinate of the end of the shape</param>
    /// <param name="character">The character to draw</param>
    /// <param name="fill"><see langword="true"/> if the shape is filled, otherwise <see langword="false"/></param>
    public NexusEllipse(in NexusCoord start, in NexusCoord end, in NexusChar character, in bool fill)
        : this(new NexusSize(end.X - start.X, end.Y - start.Y), character, fill) { }

    /// <inheritdoc/>
    public readonly bool[,] Draw()
    {
        var result = new bool[Size.Width, Size.Height];

        for (int y = 0; y < Size.Height; y++)
        {
            for (int x = 0; x < Size.Width; x++)
            {
                result[x, y] = _sprite[x, y].UnicodeChar != char.MinValue;
            }
        }

        return result;
    }

    private static ReadOnlyMemory2D<CHAR_INFO> CreateSprite(Bitmap bitmap, in NexusChar character)
    {
        var sprite = new Memory2D<CHAR_INFO>(bitmap.Width, bitmap.Height);
        var charInfo = Converter.ToCharInfo(character);

        unsafe
        {
            var data = bitmap.LockBitsReadOnly();
            var pixelSize = Image.GetPixelFormatSize(PixelFormat.Format16bppRgb555) / 8;

            var scan0 = (byte*)data.Scan0;

            byte* row;
            byte* pixel;
            for (var y = 0; y < data.Height; y++)
            {
                row = scan0 + y * data.Stride;

                for (var x = 0; x < data.Width; x++)
                {
                    pixel = row + x * pixelSize;

                    sprite[x, y] = ((pixel[1] & 0b01111100) >> 2 is 31) ? charInfo : default;
                }
            }

            bitmap.UnlockBits(data);
        }

        return sprite.ToReadOnly();
    }
}
