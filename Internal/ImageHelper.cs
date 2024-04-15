namespace ConsoleNexusEngine.Internal;

using System.Drawing;

internal static class ImageHelper
{
    public static Bitmap Resize(Bitmap bitmap, Size size)
    {
        if (size.IsEmpty) return bitmap;

        var resized = new Bitmap(size.Width, size.Height);

        using (var graphics = Graphics.FromImage(resized))
        {
            graphics.DrawImage(bitmap, 0, 0, size.Width, size.Height);
        }

        return resized;
    }

    public static Size GetSize(in int width, in int height, in float percentage)
        => percentage <= 0
        ? new Size(width, height)
        : new Size((int)(width * percentage), (int)(height * percentage));
}