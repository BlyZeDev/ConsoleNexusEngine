namespace ConsoleNexusEngine.Graphics;

using Figgle;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Represents a figgle text in the console
/// </summary>
public sealed record NexusFiggleText : ISprite
{
    private readonly ReadOnlyMemory2D<NexusChar> _sprite;

    ReadOnlyMemory2D<NexusChar> ISprite.Sprite => _sprite;

    /// <summary>
    /// The text lines itself
    /// </summary>
    public ImmutableArray<string> Value { get; }

    /// <summary>
    /// The foreground color of the text
    /// </summary>
    public NexusColorIndex Foreground { get; }

    /// <summary>
    /// The background color of the text
    /// </summary>
    public NexusColorIndex Background { get; }

    /// <summary>
    /// The figgle font of the text
    /// </summary>
    public FiggleFont FiggleFont { get; }

    /// <summary>
    /// Initializes a new console text in figgle font
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="figgleFont">The Figgle Font that is used on the <paramref name="value"/></param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="background">The background color of the text</param>
    public NexusFiggleText(string value, FiggleFont figgleFont, in NexusColorIndex foreground, in NexusColorIndex background)
    {
        var text = RenderText(value, figgleFont, out var longestLength);
        
        Value = ImmutableArray.Create(text);
        Foreground = foreground;
        Background = background;
        FiggleFont = figgleFont;

        _sprite = CreateSprite(text, foreground, background, new NexusSize(longestLength, text.Length));
    }

    /// <summary>
    /// Initializes a new console text in figgle font
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="figgleFont">The Figgle Font that is used on the <paramref name="value"/></param>
    /// <param name="foreground">The foreground color of the text</param>
    public NexusFiggleText(string value, FiggleFont figgleFont, in NexusColorIndex foreground)
        : this(value, figgleFont, foreground, NexusColorIndex.Background) { }

    /// <summary>
    /// Initializes a new console text in figgle font
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="figgleFont">The Figgle Font that is used on the <paramref name="value"/></param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="background">The background color of the text</param>
    public NexusFiggleText(in char value, FiggleFont figgleFont, in NexusColorIndex foreground, in NexusColorIndex background)
        : this(value.ToString(), figgleFont, foreground, background) { }

    /// <summary>
    /// Initializes a new console text in figgle font
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="figgleFont">The Figgle Font that is used on the <paramref name="value"/></param>
    /// <param name="foreground">The foreground color of the text</param>
    public NexusFiggleText(in char value, FiggleFont figgleFont, in NexusColorIndex foreground)
        : this(value.ToString(), figgleFont, foreground, NexusColorIndex.Background) { }

    /// <summary>
    /// Initializes a new console text in figgle font
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="figgleFont">The Figgle Font that is used on the <paramref name="value"/></param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="background">The background color of the text</param>
    public NexusFiggleText(object? value, FiggleFont figgleFont, in NexusColorIndex foreground, in NexusColorIndex background)
        : this(value?.ToString() ?? "", figgleFont, foreground, background) { }

    /// <summary>
    /// Initializes a new console text in figgle font
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="figgleFont">The Figgle Font that is used on the <paramref name="value"/></param>
    /// <param name="foreground">The foreground color of the text</param>
    public NexusFiggleText(object? value, FiggleFont figgleFont, in NexusColorIndex foreground)
        : this(value?.ToString() ?? "", figgleFont, foreground, NexusColorIndex.Background) { }

    private static ReadOnlySpan<string> RenderText(string value, FiggleFont font, out int longestLength)
    {
        var values = font.Render(value).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        longestLength = values.Aggregate("", (max, val) => val.Length > max.Length ? val : max).Length;

        return values;
    }

    private static ReadOnlyMemory2D<NexusChar> CreateSprite(in ReadOnlySpan<string> text, in NexusColorIndex foreground, in NexusColorIndex background, in NexusSize size)
    {
        var sprite = new Memory2D<NexusChar>(size.Width, size.Height);

        for (int x = 0; x < sprite.Width; x++)
        {
            for (int y = 0; y < sprite.Height; y++)
            {
                sprite[x, y] = new NexusChar(text[y][x], foreground, background);
            }
        }

        return sprite.ToReadOnly();
    }
}