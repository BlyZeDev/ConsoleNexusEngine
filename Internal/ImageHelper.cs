namespace ConsoleNexusEngine.Internal;

using System.Drawing;
using System.Drawing.Imaging;

internal static class ImageHelper
{
    public static BitmapData LockBitsReadOnly(this Bitmap bitmap)
        => bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format16bppRgb555);

    public static void UnlockBits(this Bitmap bitmap, BitmapData data) => bitmap.UnlockBits(data);

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

    public static NexusSize GetSize(in int width, in int height, in float percentage)
        => percentage <= 0
        ? new NexusSize(width, height)
        : new NexusSize((int)(width * percentage), (int)(height * percentage));
}