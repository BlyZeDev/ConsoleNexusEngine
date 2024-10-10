namespace ConsoleNexusEngine.Graphics;

using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Http;

/// <summary>
/// The color palette created from an image
/// </summary>
[IgnoreColorPalette]
public sealed class ImageColorPalette : NexusColorPalette
{
    /// <summary>
    /// Initializes a new <see cref="ImageColorPalette"/> from an image
    /// </summary>
    /// <param name="filepath">The path to the image</param>
    public ImageColorPalette(string filepath) : base(FromImage(filepath)) { }

    /// <summary>
    /// Initializes a new <see cref="ImageColorPalette"/> from an image
    /// </summary>
    /// <param name="url">The url of the image</param>
    public ImageColorPalette(Uri url) : base(FromImage(url)) { }

    /// <summary>
    /// Initializes a new <see cref="ImageColorPalette"/> from an image
    /// </summary>
    /// <param name="bitmap">The image itself</param>
    public ImageColorPalette(Bitmap bitmap) : base(FromImage(bitmap)) { }

    private static List<NexusColor> FromImage(string filepath) => FromImage((Bitmap)Image.FromFile(filepath));

    private static List<NexusColor> FromImage(Uri url)
    {
        using (var client = new HttpClient())
        {
            using (var stream = TaskHelper.RunSync(() => client.GetStreamAsync(url)))
            {
                return stream is null
                    ? throw new HttpRequestException("Couldn't read the image")
                    : FromImage(new Bitmap(stream));
            }
        }
    }

    private static List<NexusColor> FromImage(Bitmap bitmap)
    {
        var data = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

        var mostUsedColors = new Dictionary<NexusColor, int>();
        unsafe
        {
            var pixelSize = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            var scan0 = (byte*)data.Scan0;

            byte* row;
            byte* pixel;
            NexusColor color;

            for (int y = 0; y < data.Height; y++)
            {
                row = scan0 + y * data.Stride;

                for (int x = 0; x < data.Width; x++)
                {
                    pixel = row + x * pixelSize;

                    if (pixel[3] < 128) continue;

                    color = new NexusColor(pixel[2], pixel[1], pixel[0]);

                    if (!mostUsedColors.TryAdd(color, 0)) mostUsedColors[color]++;
                }
            }
        }

        bitmap.UnlockBits(data);

        var colors = new List<NexusColor>();

        foreach (var color in mostUsedColors.Keys.Take(16))
        {
            colors.Add(color);
        }

        return colors;
    }
}