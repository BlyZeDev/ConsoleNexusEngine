namespace ConsoleNexusEngine.Graphics;

using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;

/// <summary>
/// Represents an image that can be rendered in the console
/// </summary>
public readonly struct NexusImage
{
    private const char LightBlock = (char)NexusSpecialChar.LightBlock;
    private const char MiddleBlock = (char)NexusSpecialChar.MiddleBlock;
    private const char DarkBlock = (char)NexusSpecialChar.DarkBlock;
    private const char FullBlock = (char)NexusSpecialChar.FullBlock;

    private readonly ReadOnlyMemory2D<NexusChar> _pixels;

    /// <summary>
    /// The width of the image
    /// </summary>
    public readonly int Width => _pixels.Width;

    /// <summary>
    /// The height of the image
    /// </summary>
    public readonly int Height => _pixels.Height;

    internal NexusImage(Bitmap bitmap, NexusImageProcessor imageProcessor, in NexusSize? size)
        => _pixels = InitializePixels(bitmap, imageProcessor, size);

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
        : this(bitmap, imageProcessor, null) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="filepath">The path to the image file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="percentage">The desired percentage size of the bitmap</param>
    public NexusImage(string filepath, NexusImageProcessor imageProcessor, in float percentage)
        : this((Bitmap)Image.FromFile(filepath), imageProcessor, percentage) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="bitmap">The bitmap</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="percentage">The desired percentage size of the bitmap</param>
    public NexusImage(Bitmap bitmap, NexusImageProcessor imageProcessor, in float percentage)
        : this(bitmap, imageProcessor, ImageHelper.GetSize(bitmap.Width, bitmap.Height, percentage)) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="filepath">The path to the image file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="size">The desired size of the bitmap</param>
    public NexusImage(string filepath, NexusImageProcessor imageProcessor, in NexusSize size)
        : this((Bitmap)Image.FromFile(filepath), imageProcessor, size) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="bitmap">The bitmap</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="size">The desired size of the bitmap</param>
    public NexusImage(Bitmap bitmap, NexusImageProcessor imageProcessor, in NexusSize size)
        => _pixels = InitializePixels(bitmap, imageProcessor, size);

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="url">The url to the image</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <returns><see cref="NexusImage"/></returns>
    public static NexusImage FromUrl(Uri url, NexusImageProcessor imageProcessor)
    {
        using (var client = new HttpClient())
        {
            using (var stream = TaskHelper.RunSync(() => client.GetStreamAsync(url))
                ?? throw new HttpRequestException("Couldn't read the image"))
            {
                return new NexusImage(new Bitmap(stream), imageProcessor);
            }
        }
    }

    internal readonly NexusChar this[in int x, in int y] => _pixels[x, y];

    internal readonly ReadOnlyMemory2D<NexusChar> CopyPixels() => new ReadOnlyMemory2D<NexusChar>(_pixels.Span.ToArray(), Width, Height);

    private static ReadOnlyMemory2D<NexusChar> InitializePixels(Bitmap bitmap, NexusImageProcessor processor, in NexusSize? size)
    {
        var resized = ImageHelper.Resize(bitmap, size);

        var pixels = new Memory2D<NexusChar>(resized.Width, resized.Height);

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

                    pixels[x, y] = new NexusChar(GetAlphaLevel(pixel[3]),
                        processor.Process(new NexusColor(pixel[2], pixel[1], pixel[0])));
                }
            }
        }

        resized.UnlockBits(data);

        return pixels.ToReadOnly();
    }

    private static char GetAlphaLevel(in byte alpha)
    {
        return alpha switch
        {
            < 52 => char.MinValue,
            < 103 => LightBlock,
            < 154 => MiddleBlock,
            < 205 => DarkBlock,
            _ => FullBlock
        };
    }
}