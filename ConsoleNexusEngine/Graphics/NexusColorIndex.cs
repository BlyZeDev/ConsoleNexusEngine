namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents a color index between 0-15
/// </summary>
public readonly record struct NexusColorIndex
{
    /// <summary>
    /// The invalid color index
    /// </summary>
    public static NexusColorIndex Invalid => new NexusColorIndex(-1);

    /// <summary>
    /// The background color index
    /// </summary>
    public static NexusColorIndex Background => new NexusColorIndex(0);

    /// <summary>
    /// The color index 1
    /// </summary>
    public static NexusColorIndex Color1 => new NexusColorIndex(1);

    /// <summary>
    /// The color index 2
    /// </summary>
    public static NexusColorIndex Color2 => new NexusColorIndex(2);

    /// <summary>
    /// The color index 3
    /// </summary>
    public static NexusColorIndex Color3 => new NexusColorIndex(3);

    /// <summary>
    /// The color index 4
    /// </summary>
    public static NexusColorIndex Color4 => new NexusColorIndex(4);

    /// <summary>
    /// The color index 5
    /// </summary>
    public static NexusColorIndex Color5 => new NexusColorIndex(5);

    /// <summary>
    /// The color index 6
    /// </summary>
    public static NexusColorIndex Color6 => new NexusColorIndex(6);

    /// <summary>
    /// The color index 7
    /// </summary>
    public static NexusColorIndex Color7 => new NexusColorIndex(7);

    /// <summary>
    /// The color index 8
    /// </summary>
    public static NexusColorIndex Color8 => new NexusColorIndex(8);

    /// <summary>
    /// The color index 9
    /// </summary>
    public static NexusColorIndex Color9 => new NexusColorIndex(9);

    /// <summary>
    /// The color index 10
    /// </summary>
    public static NexusColorIndex Color10 => new NexusColorIndex(10);

    /// <summary>
    /// The color index 11
    /// </summary>
    public static NexusColorIndex Color11 => new NexusColorIndex(11);

    /// <summary>
    /// The color index 12
    /// </summary>
    public static NexusColorIndex Color12 => new NexusColorIndex(12);

    /// <summary>
    /// The color index 13
    /// </summary>
    public static NexusColorIndex Color13 => new NexusColorIndex(13);

    /// <summary>
    /// The color index 14
    /// </summary>
    public static NexusColorIndex Color14 => new NexusColorIndex(14);

    /// <summary>
    /// The color index 15
    /// </summary>
    public static NexusColorIndex Color15 => new NexusColorIndex(15);

    /// <summary>
    /// The color index
    /// </summary>
    public readonly int Value { get; }
    
    /// <summary>
    /// <see langword="true"/> if <see cref="Value"/> is equal to -1, otherwise <see langword="false"/>
    /// </summary>
    public readonly bool IsInvalid => Value == -1;

    /// <summary>
    /// Initializes a color index from a number
    /// </summary>
    /// <remarks>
    /// The color index is clamped between 0-15
    /// </remarks>
    /// <param name="index">The index of the color</param>
    public NexusColorIndex(int index) => Value = Math.Clamp(index, -1, 15);

    /// <summary>
    /// Initializes a <see cref="Invalid"/> index
    /// </summary>
    public NexusColorIndex() : this(-1) { }

    /// <summary>
    /// Implicitly converts <see cref="NexusColorIndex"/> to <see cref="int"/>
    /// </summary>
    /// <param name="index">The index to convert</param>
    public static implicit operator int(in NexusColorIndex index) => index.Value;
}