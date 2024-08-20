﻿namespace ConsoleNexusEngine.Graphics;

using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
/// Represents a rectangle shape
/// </summary>
public readonly struct NexusRectangle : INexusShape, ISprite
{
    private readonly ReadOnlyMemory2D<NexusChar> _sprite;

    readonly ReadOnlyMemory2D<NexusChar> ISprite.Sprite => _sprite;

    /// <inheritdoc/>
    public readonly NexusSize Size { get; }

    /// <inheritdoc/>
    public readonly NexusChar Character { get; }

    /// <inheritdoc/>
    public readonly bool Fill { get; }

    /// <summary>
    /// Initializes a new <see cref="NexusRectangle"/>
    /// </summary>
    /// <param name="size">The size of the shape</param>
    /// <param name="character">The character to draw</param>
    /// <param name="fill"><see langword="true"/> if the shape is filled, otherwise <see langword="false"/></param>
    public NexusRectangle(in NexusSize size, in NexusChar character, in bool fill)
    {
        var bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format16bppRgb555);

        Size = size;
        Character = character;
        Fill = fill;

        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.DrawRectangle(INexusShape.Red, 0, 0, size.Width - 1, size.Height - 1);
            if (Fill) graphics.FillRectangle(INexusShape.Red.Brush, 0, 0, size.Width - 1, size.Height - 1);
        }

        _sprite = CreateSprite(bitmap, Character);
    }

    /// <summary>
    /// Initializes a new <see cref="NexusRectangle"/>
    /// </summary>
    /// <param name="start">The coordinate of the start of the shape</param>
    /// <param name="end">The coordinate of the end of the shape</param>
    /// <param name="character">The character to draw</param>
    /// <param name="fill"><see langword="true"/> if the shape is filled, otherwise <see langword="false"/></param>
    public NexusRectangle(in NexusCoord start, in NexusCoord end, in NexusChar character, in bool fill)
        : this(new NexusSize(end.X - start.X, end.Y - start.Y), character, fill) { }

    /// <inheritdoc/>
    public readonly bool[,] Draw()
    {
        var result = new bool[Size.Width, Size.Height];

        for (int y = 0; y < Size.Height; y++)
        {
            for (int x = 0; x < Size.Width; x++)
            {
                result[x, y] = _sprite.Span[MathHelper.GetIndex(x, y, Size.Width)] != NexusChar.Empty;
            }
        }

        return result;
    }

    private static ReadOnlyMemory2D<NexusChar> CreateSprite(Bitmap bitmap, in NexusChar character)
    {
        var sprite = new Memory2D<NexusChar>(bitmap.Width, bitmap.Height);

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

                    sprite[x, y] = ((pixel[1] & 0b01111100) >> 2 is 31) ? character : NexusChar.Empty;
                }
            }

            bitmap.UnlockBits(data);
        }

        return sprite.ToReadOnly();
    }
}