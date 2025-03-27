namespace ConsoleNexusEngine.Graphics;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

/// <summary>
/// Represents an ellipse shape with extended features.
/// </summary>
public readonly struct NexusEllipse : INexusShape, ISprite
{
    private readonly ReadOnlyMemory2D<CHARINFO> _sprite;

    readonly ReadOnlyMemory2D<CHARINFO> ISprite.Sprite => _sprite;

    /// <inheritdoc/>
    public readonly NexusSize Size { get; }

    /// <inheritdoc/>
    public readonly NexusChar Character { get; }

    /// <inheritdoc/>
    public readonly bool Fill { get; }

    /// <summary>
    /// Gets the border character for the ellipse.
    /// </summary>
    public readonly NexusChar BorderCharacter { get; }

    /// <summary>
    /// Gets the fill character for the ellipse.
    /// </summary>
    public readonly NexusChar FillCharacter { get; }

    /// <summary>
    /// Gets the border color for the ellipse.
    /// </summary>
    public readonly Color BorderColor { get; }

    /// <summary>
    /// Gets the fill color for the ellipse.
    /// </summary>
    public readonly Color FillColor { get; }

    /// <summary>
    /// Gets the border thickness of the ellipse.
    /// </summary>
    public readonly int BorderThickness { get; }

    /// <summary>
    /// Gets a value indicating whether to draw a border.
    /// </summary>
    public readonly bool HasBorder { get; }

    /// <summary>
    /// Gets the center point of the ellipse.
    /// </summary>
    public readonly Point Center => new(Size.Width / 2, Size.Height / 2);

    /// <summary>
    /// Gets the horizontal radius of the ellipse.
    /// </summary>
    public readonly int RadiusX => Size.Width / 2;

    /// <summary>
    /// Gets the vertical radius of the ellipse.
    /// </summary>
    public readonly int RadiusY => Size.Height / 2;

    /// <summary>
    /// Initializes a new <see cref="NexusEllipse"/>.
    /// </summary>
    /// <param name="size">The size of the shape.</param>
    /// <param name="character">The character to draw for the outline (if not filled).</param>
    /// <param name="fill"><see langword="true"/> if the shape is filled, otherwise <see langword="false"/>.</param>
    /// <param name="fillCharacter">The character to use for filling the ellipse (if <paramref name="fill"/> is <see langword="true"/>).</param>
    /// <param name="fillColor">The color to use for filling the ellipse (if <paramref name="fill"/> is <see langword="true"/>).</param>
    public NexusEllipse(in NexusSize size, in NexusChar character, in bool fill, in NexusChar fillCharacter = default, in Color fillColor = default)
        : this(size, character, fill, character, Color.Red, fillCharacter, fillColor, 1, !fill) { }

    /// <summary>
    /// Initializes a new <see cref="NexusEllipse"/>.
    /// </summary>
    /// <param name="start">The coordinate of the start of the shape.</param>
    /// <param name="end">The coordinate of the end of the shape.</param>
    /// <param name="character">The character to draw for the outline (if not filled).</param>
    /// <param name="fill"><see langword="true"/> if the shape is filled, otherwise <see langword="false"/>.</param>
    /// <param name="fillCharacter">The character to use for filling the ellipse (if <paramref name="fill"/> is <see langword="true"/>).</param>
    /// <param name="fillColor">The color to use for filling the ellipse (if <paramref name="fill"/> is <see langword="true"/>).</param>
    public NexusEllipse(in NexusCoord start, in NexusCoord end, in NexusChar character, in bool fill, in NexusChar fillCharacter = default, in Color fillColor = default)
        : this(new NexusSize(end.X - start.X, end.Y - start.Y), character, fill, fillCharacter, fillColor) { }

    /// <summary>
    /// Initializes a new <see cref="NexusEllipse"/>.
    /// </summary>
    /// <param name="size">The size of the shape.</param>
    /// <param name="borderCharacter">The character to use for the border.</param>
    /// <param name="borderColor">The color of the border.</param>
    /// <param name="fillCharacter">The character to use for filling.</param>
    /// <param name="fillColor">The color of the fill.</param>
    /// <param name="borderThickness">The thickness of the border.</param>
    /// <param name="hasBorder"><see langword="true"/> to draw a border, otherwise <see langword="false"/>.</param>
    public NexusEllipse(in NexusSize size, in NexusChar borderCharacter, in Color borderColor, in NexusChar fillCharacter, in Color fillColor, int borderThickness, bool hasBorder)
        : this(size, borderCharacter, false, fillCharacter, fillColor, borderCharacter, borderColor, borderThickness, hasBorder, true) { }

    /// <summary>
    /// Initializes a new <see cref="NexusEllipse"/>.
    /// </summary>
    /// <param name="start">The coordinate of the start of the shape.</param>
    /// <param name="end">The coordinate of the end of the shape.</param>
    /// <param name="borderCharacter">The character to use for the border.</param>
    /// <param name="borderColor">The color of the border.</param>
    /// <param name="fillCharacter">The character to use for filling.</param>
    /// <param name="fillColor">The color of the fill.</param>
    /// <param name="borderThickness">The thickness of the border.</param>
    /// <param name="hasBorder"><see langword="true"/> to draw a border, otherwise <see langword="false"/>.</param>
    public NexusEllipse(in NexusCoord start, in NexusCoord end, in NexusChar borderCharacter, in Color borderColor, in NexusChar fillCharacter, in Color fillColor, int borderThickness, bool hasBorder)
        : this(new NexusSize(end.X - start.X, end.Y - start.Y), borderCharacter, borderColor, fillCharacter, fillColor, borderThickness, hasBorder) { }

    private NexusEllipse(in NexusSize size, in NexusChar character, in bool fill, in NexusChar fillCharacter, in Color fillColor, in NexusChar borderCharacter, in Color borderColor, int borderThickness, bool hasBorder, bool initialCreation)
    {
        Size = size;
        Character = character;
        Fill = fill;
        FillCharacter = fillCharacter;
        FillColor = fillColor;
        BorderCharacter = borderCharacter;
        BorderColor = borderColor;
        BorderThickness = Math.Max(0, borderThickness); // Ensure non-negative thickness
        HasBorder = hasBorder && BorderThickness > 0; // Only has a border if enabled and thickness > 0

        var bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb); // Use ARGB for color support

        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias; // Enable anti-aliasing for smoother ellipses
            var ellipseRect = new Rectangle(0, 0, size.Width - 1, size.Height - 1);

            if (Fill)
            {
                using (var fillBrush = new SolidBrush(FillColor == default ? Color.White : FillColor))
                {
                    graphics.FillEllipse(fillBrush, ellipseRect);
                }
            }

            if (HasBorder)
            {
                using (var borderPen = new Pen(BorderColor == default ? Color.Red : BorderColor, BorderThickness))
                {
                    graphics.DrawEllipse(borderPen, ellipseRect);
                }
            }
            else if (!Fill) // Draw outline if not filled and no explicit border
            {
                using (var outlinePen = new Pen(Color.Red, 1)) // Default outline color
                {
                    graphics.DrawEllipse(outlinePen, ellipseRect);
                }
            }
        }

        _sprite = CreateSprite(bitmap, Character, FillCharacter, HasBorder, BorderCharacter, Fill);
    }

    /// <inheritdoc/>
    public readonly bool[,] Draw()
    {
        var result = new bool[Size.Width, Size.Height];

        for (int y = 0; y < Size.Height; y++)
        {
            for (int x = 0; x < Size.Width; x++)
            {
                result[x, y] = _sprite[x, y].UnicodeChar != char.MinValue;
            }
        }

        return result;
    }

    /// <summary>
    /// Creates a new <see cref="NexusEllipse"/> with a different border character.
    /// </summary>
    /// <param name="newBorderCharacter">The new border character.</param>
    /// <returns>A new <see cref="NexusEllipse"/> with the updated border character.</returns>
    public NexusEllipse WithBorderCharacter(in NexusChar newBorderCharacter)
        => new(Size, newBorderCharacter, BorderColor, FillCharacter, FillColor, BorderThickness, HasBorder);

    /// <summary>
    /// Creates a new <see cref="NexusEllipse"/> with a different border color.
    /// </summary>
    /// <param name="newBorderColor">The new border color.</param>
    /// <returns>A new <see cref="NexusEllipse"/> with the updated border color.</returns>
    public NexusEllipse WithBorderColor(in Color newBorderColor)
        => new(Size, BorderCharacter, newBorderColor, FillCharacter, FillColor, BorderThickness, HasBorder);

    /// <summary>
    /// Creates a new <see cref="NexusEllipse"/> with a different fill character.
    /// </summary>
    /// <param name="newFillCharacter">The new fill character.</param>
    /// <returns>A new <see cref="NexusEllipse"/> with the updated fill character.</returns>
    public NexusEllipse WithFillCharacter(in NexusChar newFillCharacter)
        => new(Size, BorderCharacter, BorderColor, newFillCharacter, FillColor, BorderThickness, HasBorder);

    /// <summary>
    /// Creates a new <see cref="NexusEllipse"/> with a different fill color.
    /// </summary>
    /// <param name="newFillColor">The new fill color.</param>
    /// <returns>A new <see cref="NexusEllipse"/> with the updated fill color.</returns>
    public NexusEllipse WithFillColor(in Color newFillColor)
        => new(Size, BorderCharacter, BorderColor, FillCharacter, newFillColor, BorderThickness, HasBorder);

    /// <summary>
    /// Creates a new <see cref="NexusEllipse"/> with a different border thickness.
    /// </summary>
    /// <param name="newBorderThickness">The new border thickness.</param>
    /// <returns>A new <see cref="NexusEllipse"/> with the updated border thickness.</returns>
    public NexusEllipse WithBorderThickness(int newBorderThickness)
        => new(Size, BorderCharacter, BorderColor, FillCharacter, FillColor, newBorderThickness, HasBorder);

    /// <summary>
    /// Creates a new <see cref="NexusEllipse"/> with or without a border.
    /// </summary>
    /// <param name="withBorder"><see langword="true"/> to enable the border, <see langword="false"/> to disable.</param>
    /// <returns>A new <see cref="NexusEllipse"/> with the border enabled or disabled.</returns>
    public NexusEllipse WithBorder(bool withBorder)
        => new(Size, BorderCharacter, BorderColor, FillCharacter, FillColor, BorderThickness, withBorder);

    /// <summary>
    /// Creates a new <see cref="NexusEllipse"/> with a different size.
    /// </summary>
    /// <param name="newSize">The new size of the ellipse.</param>
    /// <returns>A new <see cref="NexusEllipse"/> with the updated size.</returns>
    public NexusEllipse WithSize(in NexusSize newSize)
        => new(newSize, BorderCharacter, BorderColor, FillCharacter, FillColor, BorderThickness, HasBorder);

    /// <summary>
    /// Creates a new <see cref="NexusEllipse"/> that is filled or not filled.
    /// </summary>
    /// <param name="newFill">True to make the ellipse filled, false otherwise.</param>
    /// <returns>A new <see cref="NexusEllipse"/> with the updated fill state.</returns>
    public NexusEllipse WithFill(bool newFill)
        => new(Size, BorderCharacter, BorderColor, newFill ? FillCharacter : BorderCharacter, FillColor, HasBorder ? BorderThickness : (newFill ? 0 : 1), !newFill);

    /// <summary>
    /// Creates a new <see cref="NexusEllipse"/> with a gradient fill.
    /// </summary>
    /// <param name="startColor">The starting color of the gradient.</param>
    /// <param name="endColor">The ending color of the gradient.</param>
    /// <param name="gradientMode">The direction of the gradient.</param>
    /// <param name="fillCharacter">The character to use for the fill.</param>
    /// <returns>A new <see cref="NexusEllipse"/> with a gradient fill.</returns>
    public NexusEllipse WithGradientFill(Color startColor, Color endColor, LinearGradientMode gradientMode, in NexusChar fillCharacter = default)
    {
        var bitmap = new Bitmap(Size.Width, Size.Height, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var ellipseRect = new Rectangle(0, 0, Size.Width - 1, Size.Height - 1);
            using (var gradientBrush = new LinearGradientBrush(ellipseRect, startColor, endColor, gradientMode))
            {
                graphics.FillEllipse(gradientBrush, ellipseRect);
            }
            if (HasBorder)
            {
                using (var borderPen = new Pen(BorderColor == default ? Color.Red : BorderColor, BorderThickness))
                {
                    graphics.DrawEllipse(borderPen, ellipseRect);
                }
            }
        }
        return new NexusEllipse(Size, BorderCharacter, BorderColor, fillCharacter == default ? Character : fillCharacter, Color.Transparent, BorderThickness, HasBorder)
        {
            _sprite = CreateSprite(bitmap, Character, fillCharacter == default ? Character : fillCharacter, HasBorder, BorderCharacter, true)
        };
    }

    /// <summary>
    /// Creates a new <see cref="NexusEllipse"/> with a radial gradient fill.
    /// </summary>
    /// <param name="centerColor">The color at the center of the gradient.</param>
    /// <param name="surroundColor">The color surrounding the center.</param>
    /// <param name="fillCharacter">The character to use for the fill.</param>
    /// <returns>A new <see cref="NexusEllipse"/> with a radial gradient fill.</returns>
    public NexusEllipse WithRadialGradientFill(Color centerColor, Color surroundColor, in NexusChar fillCharacter = default)
    {
        var bitmap = new Bitmap(Size.Width, Size.Height, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var ellipseRect = new Rectangle(0, 0, Size.Width - 1, Size.Height - 1);
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(ellipseRect);
                using (var gradientBrush = new PathGradientBrush(path))
                {
                    gradientBrush.CenterColor = centerColor;
                    gradientBrush.SurroundColors = new[] { surroundColor };
                    graphics.FillEllipse(gradientBrush, ellipseRect);
                }
            }
            if (HasBorder)
            {
                using (var borderPen = new Pen(BorderColor == default ? Color.Red : BorderColor, BorderThickness))
                {
                    graphics.DrawEllipse(borderPen, ellipseRect);
                }
            }
        }
        return new NexusEllipse(Size, BorderCharacter, BorderColor, fillCharacter == default ? Character : fillCharacter, Color.Transparent, BorderThickness, HasBorder)
        {
            _sprite = CreateSprite(bitmap, Character, fillCharacter == default ? Character : fillCharacter, HasBorder, BorderCharacter, true)
        };
    }

    private static ReadOnlyMemory2D<CHARINFO> CreateSprite(Bitmap bitmap, in NexusChar outlineChar, in NexusChar fillChar, bool hasBorder, in NexusChar borderChar, bool isFilled)
    {
        var sprite = new Memory2D<CHARINFO>(bitmap.Width, bitmap.Height);
        var outlineCharInfo = NativeConverter.ToCharInfo(outlineChar);
        var fillCharInfo = NativeConverter.ToCharInfo(fillChar);
        var borderCharInfo = NativeConverter.ToCharInfo(borderChar);

        unsafe
        {
            var data = bitmap.LockBitsReadOnly(PixelFormat.Format32bppArgb);
            var pixelSize = Image.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;

            var scan0 = (byte*)data.Scan0;

            for (var y = 0; y < data.Height; y++)
