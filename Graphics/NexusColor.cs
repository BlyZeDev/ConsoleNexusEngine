namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents a RGB color
/// </summary>
public readonly partial record struct NexusColor : ISpanParsable<NexusColor>, IParsable<NexusColor>
{
    private readonly uint _value;

    /// <summary>
    /// Red component of the color
    /// </summary>
    public byte R => (byte)((_value >> 16) & 0xFF);

    /// <summary>
    /// Green component of the color
    /// </summary>
    public byte G => (byte)((_value >> 8) & 0xFF);
    
    /// <summary>
    /// Blue component of the color
    /// </summary>
    public byte B => (byte)(_value & 0xFF);

    internal NexusColor(in uint color) => _value = color;

    /// <summary>
    /// Initializes a black color
    /// </summary>
    public NexusColor() : this(0x000000) { }

    /// <summary>
    /// Initializes a color from RGB
    /// </summary>
    /// <param name="r">Red component</param>
    /// <param name="g">Green component</param>
    /// <param name="b">Blue component</param>
    public NexusColor(in byte r, in byte g, in byte b) : this(FromRgb(r, g, b)) { }

    /// <summary>
    /// 1.0 is <see cref="White"/>, 0.0 is <see cref="Black"/>
    /// </summary>
    /// <returns><see cref="double"/> between 0.0 and 1.0</returns>
    public double GetLuminance()
        => (0.2126 * R + 0.7152 * G + 0.0722 * B) / 255d;

    /// <summary>
    /// Converts this color to grayscale
    /// </summary>
    /// <returns><see cref="NexusColor"/></returns>
    public NexusColor ToGrayscale()
    {
        var gray = (byte)Math.Clamp(R * 0.3 + G * 0.59 + B * 0.11, byte.MinValue, byte.MaxValue);
        return new(gray, gray, gray);
    }

    /// <summary>
    /// Format: "[R={<see cref="R"/>},G={<see cref="G"/>},B={<see cref="B"/>}]"
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
        => $"[R={R},G={G},B={B}]";
}