namespace ConsoleNexusEngine.Common;

using System;

/// <summary>
/// Represents a RGB color
/// </summary>
public readonly partial record struct NexusColor : ISpanParsable<NexusColor>, IParsable<NexusColor>
{
    /// <summary>
    /// The color formatted as RRGGBB
    /// </summary>
    public uint Color { get; }

    /// <summary>
    /// Red component of the color
    /// </summary>
    public byte R => (byte)((Color >> 16) & 0xFF);

    /// <summary>
    /// Green component of the color
    /// </summary>
    public byte G => (byte)((Color >> 8) & 0xFF);
    
    /// <summary>
    /// Blue component of the color
    /// </summary>
    public byte B => (byte)(Color & 0xFF);

    private NexusColor(in uint color) => Color = color;

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
        => 0.299 * R + 0.587 * G + 0.114 * B;

    /// <summary>
    /// Format: "[R={<see cref="R"/>},G={<see cref="G"/>},B={<see cref="B"/>}]"
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
        => $"[R={R},G={G},B={B}]";
}