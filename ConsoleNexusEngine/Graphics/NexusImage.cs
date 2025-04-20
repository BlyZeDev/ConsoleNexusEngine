namespace ConsoleNexusEngine.Graphics;

using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;

/// <summary>
/// Represents an image that can be rendered in the console
/// </summary>
public readonly struct NexusImage : INexusSprite
{
    private const char LightBlock = '░';
    private const char MiddleBlock = '▒';
    private const char DarkBlock = '▓';
    private const char FullBlock = '█';

    /// <inheritdoc/>
    public readonly NexusSpriteMap Sprite { get; }

    internal NexusImage(Bitmap bitmap, NexusColorProcessor imageProcessor, in NexusSize? size)
        => Sprite = CreateSprite(bitmap, imageProcessor, size);

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="filepath">The path to the image file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    public NexusImage(string filepath, NexusColorProcessor imageProcessor)
        : this((Bitmap)Image.FromFile(filepath), imageProcessor) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="bitmap">The bitmap</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    public NexusImage(Bitmap bitmap, NexusColorProcessor imageProcessor)
        : this(bitmap, imageProcessor, null) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="filepath">The path to the image file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="percentage">The desired percentage size of the bitmap</param>
    public NexusImage(string filepath, NexusColorProcessor imageProcessor, float percentage)
        : this((Bitmap)Image.FromFile(filepath), imageProcessor, percentage) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="bitmap">The bitmap</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="percentage">The desired percentage size of the bitmap</param>
    public NexusImage(Bitmap bitmap, NexusColorProcessor imageProcessor, float percentage)
        : this(bitmap, imageProcessor, ImageHelper.GetSize(bitmap.Width, bitmap.Height, percentage)) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="filepath">The path to the image file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="size">The desired size of the bitmap</param>
    public NexusImage(string filepath, NexusColorProcessor imageProcessor, in NexusSize size)
        : this((Bitmap)Image.FromFile(filepath), imageProcessor, size) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="bitmap">The bitmap</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="size">The desired size of the bitmap</param>
    public NexusImage(Bitmap bitmap, NexusColorProcessor imageProcessor, in NexusSize size)
        : this(bitmap, imageProcessor, new NexusSize?(size)) { }

    /// <summary>
    /// Initializes a new NexusImage
    /// </summary>
    /// <param name="url">The url to the image</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <returns><see cref="NexusImage"/></returns>
    public static NexusImage FromUrl(Uri url, NexusColorProcessor imageProcessor)
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

    private static NexusSpriteMap CreateSprite(Bitmap bitmap, NexusColorProcessor processor, in NexusSize? size)
    {
        var resized = ImageHelper.Resize(bitmap, size);

        var sprite = new NexusWritableSpriteMap(resized.Width, resized.Height);

        var data = resized.LockBits(
            new Rectangle(0, 0, resized.Width, resized.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

        try
        {
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

                        sprite._spriteMap[IndexDimensions.Get1D(x, y, sprite.Width)] = NativeConverter.ToCharInfo(
                            GetAlphaLevel(pixel[3]),
                            processor.Process(new NexusColor(pixel[2], pixel[1], pixel[0])),
                            0);
                    }
                }
            }
        }
        finally
        {
            resized.UnlockBits(data);
        }

        return sprite.AsReadOnly();
    }

    private static char GetAlphaLevel(byte alpha)
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