namespace ConsoleNexusEngine.Graphics;

using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
/// Represents a sprite that can be rendered
/// </summary>
public readonly record struct NexusSprite
{
    private readonly ReadOnlyMemory2D<NexusChar> _render;

    /// <summary>
    /// The size of the sprite
    /// </summary>
    public readonly NexusSize Size => new NexusSize(_render.Width, _render.Height);

    private NexusSprite(in ReadOnlyMemory2D<NexusChar> render) => _render = render;

    /// <summary>
    /// Initializes a new <see cref="NexusSprite"/> from <see cref="NexusChar"/>
    /// </summary>
    /// <param name="character">The character to use as sprite</param>
    public NexusSprite(in NexusChar character) : this(CreateRender(character)) { }

    /// <summary>
    /// Initializes a new <see cref="NexusSprite"/> from <see cref="NexusImage"/>
    /// </summary>
    /// <param name="image">The image to use as sprite</param>
    public NexusSprite(in NexusImage image) : this(image.CopyPixels()) { }

    /// <summary>
    /// Initializes a new <see cref="NexusSprite"/> from <see cref="INexusShape"/>
    /// </summary>
    /// <param name="shape">The shape to use as sprite</param>
    public NexusSprite(INexusShape shape) : this(CreateRender(shape)) { }

    internal readonly NexusChar this[in int x, in int y] => _render[x, y];

    private static ReadOnlyMemory2D<NexusChar> CreateRender(in NexusChar character)
    {
        var render = new Memory2D<NexusChar>(1, 1);
        render[0] = character;
        return render.ToReadOnly();
    }

    private static ReadOnlyMemory2D<NexusChar> CreateRender(INexusShape shape)
    {
        var render = new Memory2D<NexusChar>(shape.Size.Width, shape.Size.Height);
        
        switch (shape)
        {
            case NexusRectangle rectangle: RenderRectangle(rectangle, ref render); break;
            case NexusEllipse ellipse: RenderEllipse(ellipse, ref render); break;

            default:
                var drawable = shape.Draw();

                for (int x = 0; x < drawable.GetLength(0); x++)
                {
                    for (int y = 0; y < drawable.GetLength(1); y++)
                    {
                        if (drawable[x, y]) render[x, y] = shape.Character;
                    }
                }
                break;
        }

        return render.ToReadOnly();
    }

    private static void RenderRectangle(in NexusRectangle rectangle, ref Memory2D<NexusChar> render)
    {
        unsafe
        {
            var data = rectangle.LockBitsReadOnly();
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

                    if ((pixel[1] & 0b01111100) >> 2 is 31) render[x, y] = rectangle.Character;
                }
            }

            rectangle.UnlockBits(data);
        }
    }

    private static void RenderEllipse(in NexusEllipse ellipse, ref Memory2D<NexusChar> render)
    {
        unsafe
        {
            var data = ellipse.LockBitsReadOnly();
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

                    if ((pixel[1] & 0b01111100) >> 2 is 31) render[x, y] = ellipse.Character;
                }
            }

            ellipse.UnlockBits(data);
        }
    }
}