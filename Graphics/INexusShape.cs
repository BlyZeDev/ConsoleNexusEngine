namespace ConsoleNexusEngine.Graphics;

using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
/// Represents an interface for a shape
/// </summary>
public interface INexusShape
{
    internal static Pen Red => Pens.Red;

    /// <summary>
    /// The size of the shape
    /// </summary>
    public NexusSize Size { get; }

    /// <summary>
    /// The character to draw
    /// </summary>
    public NexusChar Character { get; }

    /// <summary>
    /// <see langword="true"/> if the shape is filled, otherwise <see langword="false"/>
    /// </summary>
    public bool Fill { get; }

    /// <summary>
    /// Draws on the bitmap and returns it
    /// </summary>
    /// <returns><see cref="Bitmap"/></returns>
    public Bitmap Draw();

    /// <inheritdoc cref="Bitmap.LockBits(Rectangle, ImageLockMode, PixelFormat)"/>
    /// <returns><see cref="BitmapData"/></returns>
    public BitmapData LockBitsReadOnly();

    /// <inheritdoc cref="Bitmap.UnlockBits(BitmapData)"/>
    /// <param name="data">The data of the locked bitmap</param>
    public void UnlockBits(BitmapData data);
}