namespace ConsoleNexusEngine.Graphics;

using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
/// Represents an image that can be rendered as ascii art
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
    /// <param name="size">The desired size of the bitmap</param>
    public NexusImage(string filepath, NexusImageProcessor imageProcessor, Size? size = null) : this((Bitmap)Image.FromFile(filepath), imageProcessor, size) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="image">The bitmap</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="size">The desired size of the bitmap</param>
    public NexusImage(Bitmap image, NexusImageProcessor imageProcessor, Size? size = null) => _pixels = InitializePixels(image, imageProcessor, size ?? Size.Empty);

    private static NexusChar[,] InitializePixels(Bitmap bitmap, NexusImageProcessor processor, Size size)
    {
        var temp = bitmap;

        if (!size.IsEmpty)
        {
            temp = new Bitmap(size.Width, size.Height);

            using (var graphics = Graphics.FromImage(temp))
            {
                graphics.DrawImage(bitmap, 0, 0, size.Width, size.Height);
            }
        }

        var pixels = new NexusChar[temp.Width, temp.Height];

        var data = temp.LockBits(
            new Rectangle(0, 0, temp.Width, temp.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);
        
        unsafe
        {
            var pixelSize = Image.GetPixelFormatSize(temp.PixelFormat) / 8;
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

        temp.UnlockBits(data);

        return pixels;
    }
}