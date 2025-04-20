namespace ConsoleNexusEngine.Graphics;

using System.Linq;

/// <summary>
/// Represents a text in the console
/// </summary>
public sealed record NexusText : INexusSprite
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public NexusSpriteMap Map { get; }

    /// <summary>
    /// The text itself
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// The foreground color of the text
    /// </summary>
    public NexusColorIndex Foreground { get; }

    /// <summary>
    /// The background color of the text
    /// </summary>
    public NexusColorIndex Background { get; }

    /// <summary>
    /// The flow direction of the text
    /// </summary>
    public NexusTextDirection TextDirection { get; }

    /// <summary>
    /// Initializes a new console text
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="background">The background color of the text</param>
    /// <param name="textDirection">The flow direction of the text</param>
    public NexusText(string value, in NexusColorIndex foreground, in NexusColorIndex background, NexusTextDirection textDirection = NexusTextDirection.Horizontal)
    {
        Value = value;
        Foreground = foreground;
        Background = background;
        TextDirection = textDirection;

        Map = CreateSprite(value, Foreground, Background, TextDirection);
    }

    /// <summary>
    /// Initializes a new console text
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="textDirection">The flow direction of the text</param>
    public NexusText(string value, in NexusColorIndex foreground, NexusTextDirection textDirection = NexusTextDirection.Horizontal)
        : this(value, foreground, NexusColorIndex.Background, textDirection) { }

    /// <summary>
    /// Initializes a new console text
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="background">The background color of the text</param>
    /// <param name="textDirection">The flow direction of the text</param>
    public NexusText(object? value, in NexusColorIndex foreground, in NexusColorIndex background, NexusTextDirection textDirection = NexusTextDirection.Horizontal)
        : this(value?.ToString() ?? "", foreground, background, textDirection) { }

    /// <summary>
    /// Initializes a new console text
    /// </summary>
    /// <param name="value">The text itself</param>
    /// <param name="foreground">The foreground color of the text</param>
    /// <param name="textDirection">The flow direction of the text</param>
    public NexusText(object? value, in NexusColorIndex foreground, NexusTextDirection textDirection = NexusTextDirection.Horizontal)
        : this(value?.ToString() ?? "", foreground, NexusColorIndex.Background, textDirection) { }

    private static NexusSpriteMap CreateSprite(string value, in NexusColorIndex foreground, in NexusColorIndex background, NexusTextDirection direction)
    {
        var isHorizontal = direction is NexusTextDirection.Horizontal or NexusTextDirection.HorizontalRightToLeft;

        var sprite = new NexusWritableSpriteMap(new NexusSize(isHorizontal ? value.Length : 1, isHorizontal ? 1 : value.Length));

        var text = direction is NexusTextDirection.HorizontalRightToLeft or NexusTextDirection.VerticalRightToLeft ? value.Reverse() : value;

        var index = -1;
        foreach (var letter in text)
        {
            index++;
            sprite._spriteMap[index] = NativeConverter.ToCharInfo(letter, foreground, background);
        }

        return sprite.AsReadOnly();
    }
}