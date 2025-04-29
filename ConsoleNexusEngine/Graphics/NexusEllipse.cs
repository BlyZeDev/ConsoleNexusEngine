namespace ConsoleNexusEngine.Graphics;

using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
/// Represents an ellipse shape
/// </summary>
public readonly struct NexusEllipse : INexusSprite
{
    /// <inheritdoc/>
    public readonly NexusSpriteMap Map { get; }

    /// <summary>
    /// Initializes a new <see cref="NexusEllipse"/>
    /// </summary>
    /// <param name="size">The size of the shape</param>
    /// <param name="character">The character to draw</param>
    /// <param name="fill"><see langword="true"/> if the shape is filled, otherwise <see langword="false"/></param>
    public NexusEllipse(in NexusSize size, in NexusChar character, bool fill)
    {
        ArgumentOutOfRangeException.ThrowIfZero(size.Width, nameof(size.Width));
        ArgumentOutOfRangeException.ThrowIfZero(size.Height, nameof(size.Height));

        var bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format16bppRgb555);

        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.DrawEllipse(Pens.Red, 0, 0, size.Width - 1, size.Height - 1);
            if (fill) graphics.FillEllipse(Pens.Red.Brush, 0, 0, size.Width - 1, size.Height - 1);
        }

        Map = CreateSprite(bitmap, character);
    }

    /// <summary>
    /// Initializes a new <see cref="NexusEllipse"/>
    /// </summary>
    /// <param name="start">The coordinate of the start of the shape</param>
    /// <param name="end">The coordinate of the end of the shape</param>
    /// <param name="character">The character to draw</param>
    /// <param name="fill"><see langword="true"/> if the shape is filled, otherwise <see langword="false"/></param>
    public NexusEllipse(in NexusCoord start, in NexusCoord end, in NexusChar character, bool fill)
        : this(new NexusSize(end.X - start.X, end.Y - start.Y), character, fill) { }

    private static NexusSpriteMap CreateSprite(Bitmap bitmap, in NexusChar character)
    {
        var size = new NexusSize(bitmap.Width, bitmap.Height);

        Span<CHARINFO> sprite = StackAlloc.Allow<CHARINFO>(size.Dimensions) ? stackalloc CHARINFO[size.Dimensions] : new CHARINFO[size.Dimensions];
        var charInfo = NativeConverter.ToCharInfo(character);

        unsafe
        {
            var data = bitmap.LockBitsReadOnly(PixelFormat.Format16bppRgb555);
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

                    sprite[IndexDimensions.Get1D(x, y, size.Width)] = ((pixel[1] & 0b01111100) >> 2 is 31) ? charInfo : default;
                }
            }

            bitmap.UnlockBits(data);
        }

        return new NexusSpriteMap(sprite, size);
    }
}
