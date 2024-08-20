namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Represents a color index between 0-15
/// </summary>
public readonly record struct NexusColorIndex
{
    /// <summary>
    /// The invalid color index
    /// </summary>
    public static NexusColorIndex Invalid => UnClamped(-1);

    /// <summary>
    /// The background color index
    /// </summary>
    public static NexusColorIndex Background => UnClamped(0);

    /// <summary>
    /// The color index 1
    /// </summary>
    public static NexusColorIndex Color1 => UnClamped(1);

    /// <summary>
    /// The color index 2
    /// </summary>
    public static NexusColorIndex Color2 => UnClamped(2);

    /// <summary>
    /// The color index 3
    /// </summary>
    public static NexusColorIndex Color3 => UnClamped(3);

    /// <summary>
    /// The color index 4
    /// </summary>
    public static NexusColorIndex Color4 => UnClamped(4);

    /// <summary>
    /// The color index 5
    /// </summary>
    public static NexusColorIndex Color5 => UnClamped(5);

    /// <summary>
    /// The color index 6
    /// </summary>
    public static NexusColorIndex Color6 => UnClamped(6);

    /// <summary>
    /// The color index 7
    /// </summary>
    public static NexusColorIndex Color7 => UnClamped(7);

    /// <summary>
    /// The color index 8
    /// </summary>
    public static NexusColorIndex Color8 => UnClamped(8);

    /// <summary>
    /// The color index 9
    /// </summary>
    public static NexusColorIndex Color9 => UnClamped(9);

    /// <summary>
    /// The color index 10
    /// </summary>
    public static NexusColorIndex Color10 => UnClamped(10);

    /// <summary>
    /// The color index 11
    /// </summary>
    public static NexusColorIndex Color11 => UnClamped(11);

    /// <summary>
    /// The color index 12
    /// </summary>
    public static NexusColorIndex Color12 => UnClamped(12);

    /// <summary>
    /// The color index 13
    /// </summary>
    public static NexusColorIndex Color13 => UnClamped(13);

    /// <summary>
    /// The color index 14
    /// </summary>
    public static NexusColorIndex Color14 => UnClamped(14);

    /// <summary>
    /// The color index 15
    /// </summary>
    public static NexusColorIndex Color15 => UnClamped(15);

    /// <summary>
    /// The color index
    /// </summary>
    public readonly int Index { get; }
    
    /// <summary>
    /// <see langword="true"/> if <see cref="Index"/> is equal to -1, otherwise <see langword="false"/>
    /// </summary>
    public readonly bool IsInvalid => Index is -1;

    private NexusColorIndex(int index) => Index = index;

    /// <summary>
    /// Initializes a color index from a number
    /// </summary>
    /// <remarks>
    /// The color index is clamped between 0-15
    /// </remarks>
    /// <param name="index">The index of the color</param>
    public NexusColorIndex(in int index) => Index = Math.Clamp(index, 0, 15);

    /// <summary>
    /// Initializes a <see cref="Invalid"/> index
    /// </summary>
    public NexusColorIndex() : this(-1) { }

    /// <summary>
    /// Implicitly converts <see cref="NexusColorIndex"/> to <see cref="int"/>
    /// </summary>
    /// <param name="index">The index to convert</param>
    public static implicit operator int(in NexusColorIndex index) => index.Index;

    internal static NexusColorIndex UnClamped(in int index) => new NexusColorIndex(index);
}