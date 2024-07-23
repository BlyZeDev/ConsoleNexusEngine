namespace ConsoleNexusEngine;

using System.Drawing;
using System.Drawing.Imaging;

public sealed partial class ConsoleGraphic
{
    private void DrawShape(in NexusCoord start, INexusShape shape, in Glyph glyph)
    {
        if (shape is ILockablePixels lockable) DrawLockable(start, lockable, glyph);

        var drawable = shape.Draw();

        for (int x = 0; x < drawable.GetLength(0); x++)
        {
            for (int y = 0; y < drawable.GetLength(1); y++)
            {
                if (drawable[x, y]) SetGlyph(x + start.X, y + start.Y, glyph);
            }
        }
    }

    private void DrawLockable(in NexusCoord start, in ILockablePixels lockable, in Glyph glyph)
    {
        unsafe
        {
            var data = lockable.LockBitsReadOnly();
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

                    if ((pixel[1] & 0b01111100) >> 2 is 31) SetGlyph(x + start.X, y + start.Y, glyph);
                }
            }

            lockable.UnlockBits(data);
        }
    }

    private void SetGlyph(in NexusCoord coord, in Glyph glyph)
        => SetGlyph(coord.X, coord.Y, glyph);

    private void SetGlyph(in int x, in int y, in Glyph glyph)
    {
        glyphBuffer[x, y] = glyph;
        _console.Buffer.SetGlyph(x, y, glyph);
    }

    private void ThrowIfOutOfBounds(in NexusCoord coord)
    {
        if (!glyphBuffer.IsInRange(coord))
            throw new ArgumentOutOfRangeException(nameof(coord), "The coordinate is not in bounds of the console buffer");
    }

    private void GetOrThrowColorIndex(in NexusColor foreground, in NexusColor? background, string paramName, out int foregroundColorIndex, out int backgroundColorIndex)
    {
        foregroundColorIndex = GetColorIndex(foreground);
        backgroundColorIndex = background is null ? BackgroundIndex : GetColorIndex(background.Value);

        if (foregroundColorIndex is -1 || backgroundColorIndex is -1)
            throw new ArgumentException("The color is not in the color palette", paramName);
    }

    private Glyph GetClearGlyph() => new(char.MinValue, BackgroundIndex, BackgroundIndex);

    private int GetColorIndex(in NexusColor color)
        => _settings.ColorPalette.Colors.GetKey(color);
}