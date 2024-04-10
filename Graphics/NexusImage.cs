namespace ConsoleNexusEngine.Graphics;

using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
/// Represents an image that can be rendered in the console
/// </summary>
public readonly struct NexusImage
{
    private const char FullBlock = (char)NexusSpecialChar.FullBlock;

    internal readonly NexusChar[,] _pixels;

    /// <summary>
    /// The width of the image
    /// </summary>
    public int Width => _pixels.GetLength(0);

    /// <summary>
    /// The height of the image
    /// </summary>
    public int Height => _pixels.GetLength(1);

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
        : this(bitmap, imageProcessor, GetSize(bitmap.Width, bitmap.Height, percentage)) { }

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

    private static NexusChar[,] InitializePixels(Bitmap bitmap, NexusImageProcessor processor, Size size)
    {
        var resized = Resize(bitmap, size);

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

    private static Bitmap Resize(Bitmap bitmap, Size size)
    {
        if (size.IsEmpty) return bitmap;

        var resized = new Bitmap(size.Width, size.Height);

        using (var graphics = Graphics.FromImage(resized))
        {
            graphics.DrawImage(bitmap, 0, 0, size.Width, size.Height);
        }

        return resized;
    }

    private static Size GetSize(in int width, in int height, in float percentage)
        => percentage <= 0
        ? new Size(width, height)
        : new Size((int)(width * percentage), (int)(height * percentage));
}