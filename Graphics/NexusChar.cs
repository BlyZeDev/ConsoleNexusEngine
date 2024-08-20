namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents a character in the console
/// </summary>
public readonly record struct NexusChar : ISprite
{
    /// <summary>
    /// Represents an empty <see cref="NexusChar"/>
    /// </summary>
    public static NexusChar Empty => new NexusChar();

    private readonly ReadOnlyMemory2D<NexusChar> _sprite;

    readonly ReadOnlyMemory2D<NexusChar> ISprite.Sprite => _sprite;

    /// <summary>
    /// The character itself
    /// </summary>
    public readonly char Value { get; }

    /// <summary>
    /// The foreground color of the text
    /// </summary>
    public readonly NexusColorIndex Foreground { get; }

    /// <summary>
    /// The background color of the text
    /// </summary>
    public readonly NexusColorIndex Background { get; }

    /// <summary>
    /// Initializes a new console character
    /// </summary>
    public NexusChar() : this(char.MinValue, NexusColorIndex.Background, NexusColorIndex.Background) { }

    /// <summary>
    /// Initializes a new console character
    /// </summary>
    /// <param name="value">The character itself</param>
    /// <param name="foreground">The foreground color of the character</param>
    /// <param name="background">The background color of the character</param>
    public NexusChar(in char value, in NexusColorIndex foreground, in NexusColorIndex background)
    {
        Value = value;
        Foreground = foreground;
        Background = background;

        _sprite = new ReadOnlyMemory2D<NexusChar>(this);
    }

    /// <summary>
    /// Initializes a new console character
    /// </summary>
    /// <param name="value">The character itself</param>
    /// <param name="foreground">The foreground color of the character</param>
    public NexusChar(in char value, in NexusColorIndex foreground)
        : this(value, foreground, NexusColorIndex.Background) { }

    /// <summary>
    /// Initializes a new console character
    /// </summary>
    /// <param name="value">The character itself</param>
    /// <param name="foreground">The foreground color of the character</param>
    /// <param name="background">The background color of the character</param>
    public NexusChar(in NexusSpecialChar value, in NexusColorIndex foreground, in NexusColorIndex background)
        : this((char)value, foreground, background) { }

    /// <summary>
    /// Initializes a new console character
    /// </summary>
    /// <param name="value">The character itself</param>
    /// <param name="foreground">The foreground color of the character</param>
    public NexusChar(in NexusSpecialChar value, in NexusColorIndex foreground)
        : this((char)value, foreground, NexusColorIndex.Background) { }
}