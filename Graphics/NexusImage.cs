﻿namespace ConsoleNexusEngine.Graphics;

using CommunityToolkit.HighPerformance;
using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
/// Represents an image that can be rendered in the console
/// </summary>
public readonly struct NexusImage
{
    private const char FullBlock = (char)NexusSpecialChar.FullBlock;

    private readonly ReadOnlyMemory2D<NexusChar> _pixels;

    /// <summary>
    /// The width of the image
    /// </summary>
    public int Width => _pixels.Height;

    /// <summary>
    /// The height of the image
    /// </summary>
    public int Height => _pixels.Width;

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="filepath">The path to the image file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    public NexusImage(string filepath, NexusImageProcessor imageProcessor)
        : this((Bitmap)Image.FromFile(filepath), imageProcessor) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="bitmap">The bitmap</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    public NexusImage(Bitmap bitmap, NexusImageProcessor imageProcessor)
        : this(bitmap, imageProcessor, Size.Empty) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="filepath">The path to the image file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="percentage">The desired percentage size of the bitmap</param>
    public NexusImage(string filepath, NexusImageProcessor imageProcessor, float percentage)
        : this((Bitmap)Image.FromFile(filepath), imageProcessor, percentage) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="bitmap">The bitmap</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="percentage">The desired percentage size of the bitmap</param>
    public NexusImage(Bitmap bitmap, NexusImageProcessor imageProcessor, float percentage)
        : this(bitmap, imageProcessor, ImageHelper.GetSize(bitmap.Width, bitmap.Height, percentage)) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="filepath">The path to the image file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="size">The desired size of the bitmap</param>
    public NexusImage(string filepath, NexusImageProcessor imageProcessor, Size size)
        : this((Bitmap)Image.FromFile(filepath), imageProcessor, size) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="bitmap">The bitmap</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="size">The desired size of the bitmap</param>
    public NexusImage(Bitmap bitmap, NexusImageProcessor imageProcessor, Size size)
        => _pixels = InitializePixels(bitmap, imageProcessor, size);

    internal NexusChar this[in int x, in int y] => _pixels.Span[x, y];

    private static ReadOnlyMemory2D<NexusChar> InitializePixels(Bitmap bitmap, NexusImageProcessor processor, Size size)
    {
        var resized = ImageHelper.Resize(bitmap, size);

        var pixels = new NexusChar[resized.Width, resized.Height];

        var data = resized.LockBits(
            new Rectangle(0, 0, resized.Width, resized.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);
        
        unsafe
        {
            var pixelSize = Image.GetPixelFormatSize(resized.PixelFormat) / 8;
            var scan0 = (byte*)data.Scan0;

            byte* row;
            byte* pixel;

            for (int y = 0; y < data.Height; y++)
            {
                row = scan0 + y * data.Stride;

                for (int x = 0; x < data.Width; x++)
                {
                    pixel = row + x * pixelSize;

                    pixels[x, y] = pixel[3] < 128
                        ? new NexusChar(char.MinValue, processor._colorPalette.Color1)
                        : new NexusChar(FullBlock, processor.Process(new NexusColor(pixel[2], pixel[1], pixel[0])));
                }
            }
        }

        resized.UnlockBits(data);

        return pixels;
    }
}