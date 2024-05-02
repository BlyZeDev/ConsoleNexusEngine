namespace ConsoleNexusEngine.Graphics;

using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
/// Represents an ellipse shape
/// </summary>
public readonly struct NexusEllipse : INexusShape
{
    private readonly Bitmap _bitmap;

    /// <inheritdoc/>
    public NexusSize Size { get; }

    /// <inheritdoc/>
    public readonly NexusChar Character { get; }

    /// <inheritdoc/>
    public readonly bool Fill { get; }

    /// <summary>
    /// Initializes a new <see cref="NexusEllipse"/>
    /// </summary>
    /// <param name="size">The size of the shape</param>
    /// <param name="character">The character to draw</param>
    /// <param name="fill"><see langword="true"/> if the shape is filled, otherwise <see langword="false"/></param>
    public NexusEllipse(in NexusSize size, in NexusChar character, in bool fill)
    {
        _bitmap = new Bitmap(size.Width + 1, size.Height + 1, PixelFormat.Format16bppRgb555);

        Size = size;
        Character = character;
        Fill = fill;

        using (var graphics = Graphics.FromImage(_bitmap))
        {
            graphics.DrawEllipse(INexusShape.Red, 0, 0, size.Width, size.Height);
            if (Fill) graphics.FillEllipse(INexusShape.Red.Brush, 0, 0, size.Width, size.Height);
        }
    }

    /// <summary>
    /// Initializes a new <see cref="NexusEllipse"/>
    /// </summary>
    /// <param name="start">The coordinate of the start of the shape</param>
    /// <param name="end">The coordinate of the end of the shape</param>
    /// <param name="character">The character to draw</param>
    /// <param name="fill"><see langword="true"/> if the shape is filled, otherwise <see langword="false"/></param>
    public NexusEllipse(in NexusCoord start, in NexusCoord end, in NexusChar character, in bool fill)
        : this(new NexusSize(end.X - start.X, end.Y - start.Y), character, fill) { }

    /// <inheritdoc/>
    public readonly Bitmap Draw() => _bitmap;

    /// <inheritdoc/>
    public readonly BitmapData LockBitsReadOnly()
        => _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format16bppRgb555);
    
    /// <inheritdoc/>
    public readonly void UnlockBits(BitmapData data) => _bitmap.UnlockBits(data);
}
