namespace ConsoleNexusEngine.Internal;

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
internal static class ImageHelper
{
    public static BitmapData LockBitsReadOnly(this Bitmap bitmap, PixelFormat format)
        => bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, format);

    public static Bitmap Resize(Bitmap bitmap, in NexusSize? size)
    {
        if (!size.HasValue) return bitmap;

        var resized = new Bitmap(size.Value.Width, size.Value.Height);

        using (var graphics = Graphics.FromImage(resized))
        {
            graphics.DrawImage(bitmap, 0, 0, size.Value.Width, size.Value.Height);
        }

        return resized;
    }

    public static NexusSize? GetSize(int width, int height, float percentage)
        => percentage <= 0 ? null : new NexusSize((int)(width * percentage), (int)(height * percentage));
}