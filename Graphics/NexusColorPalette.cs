namespace ConsoleNexusEngine.Graphics;

using System.Collections;

/// <summary>
/// Represents a color palette with 16 unique colors for the console
/// </summary>
public sealed partial class NexusColorPalette : IEnumerable<NexusColor>, IEquatable<NexusColorPalette>
{
    private readonly UniqueDictionary<ConsoleColor, NexusColor> _colors;

    internal IReadOnlyDictionary<ConsoleColor, NexusColor> Colors => _colors.AsReadOnly();

    /// <summary>
    /// 1st Color of the Palette
    /// </summary>
    /// <remarks>
    /// This color is always the background color
    /// </remarks>
    public NexusColor Color1 => Colors[0];

    /// <summary>
    /// 2nd Color of the Palette
    /// </summary>
    public NexusColor Color2 => Colors[(ConsoleColor)1];

    /// <summary>
    /// 3rd Color of the Palette
    /// </summary>
    public NexusColor Color3 => Colors[(ConsoleColor)2];

    /// <summary>
    /// 4th Color of the Palette
    /// </summary>
    public NexusColor Color4 => Colors[(ConsoleColor)3];

    /// <summary>
    /// 5th Color of the Palette
    /// </summary>
    public NexusColor Color5 => Colors[(ConsoleColor)4];

    /// <summary>
    /// 6th Color of the Palette
    /// </summary>
    public NexusColor Color6 => Colors[(ConsoleColor)5];

    /// <summary>
    /// 7th Color of the Palette
    /// </summary>
    public NexusColor Color7 => Colors[(ConsoleColor)6];

    /// <summary>
    /// 8th Color of the Palette
    /// </summary>
    public NexusColor Color8 => Colors[(ConsoleColor)7];

    /// <summary>
    /// 9th Color of the Palette
    /// </summary>
    public NexusColor Color9 => Colors[(ConsoleColor)8];

    /// <summary>
    /// 10th Color of the Palette
    /// </summary>
    public NexusColor Color10 => Colors[(ConsoleColor)9];

    /// <summary>
    /// 11th Color of the Palette
    /// </summary>
    public NexusColor Color11 => Colors[(ConsoleColor)10];

    /// <summary>
    /// 12th Color of the Palette
    /// </summary>
    public NexusColor Color12 => Colors[(ConsoleColor)11];

    /// <summary>
    /// 13th Color of the Palette
    /// </summary>
    public NexusColor Color13 => Colors[(ConsoleColor)12];

    /// <summary>
    /// 14th Color of the Palette
    /// </summary>
    public NexusColor Color14 => Colors[(ConsoleColor)13];

    /// <summary>
    /// 15th Color of the Palette
    /// </summary>
    public NexusColor Color15 => Colors[(ConsoleColor)14];

    /// <summary>
    /// 16th Color of the Palette
    /// </summary>
    public NexusColor Color16 => Colors[(ConsoleColor)15];

    internal NexusColorPalette(in ReadOnlySpan<NexusColor> colors)
    {
        if (colors.Length is not 16)
            throw new ArgumentException("The color palette must contain exactly 16 colors");

        _colors = [];
        for (int i = 0; i < 16; i++)
        {
            _colors.Add((ConsoleColor)i, colors[i]);
        }
    }

    /// <summary>
    /// Initializes a new Color Palette with 16 colors
    /// </summary>
    /// <param name="color1">1st Color of the Palette</param>
    /// <param name="color2">2nd Color of the Palette</param>
    /// <param name="color3">3rd Color of the Palette</param>
    /// <param name="color4">4th Color of the Palette</param>
    /// <param name="color5">5th Color of the Palette</param>
    /// <param name="color6">6th Color of the Palette</param>
    /// <param name="color7">7th Color of the Palette</param>
    /// <param name="color8">8th Color of the Palette</param>
    /// <param name="color9">9th Color of the Palette</param>
    /// <param name="color10">10th Color of the Palette</param>
    /// <param name="color11">11th Color of the Palette</param>
    /// <param name="color12">12th Color of the Palette</param>
    /// <param name="color13">13th Color of the Palette</param>
    /// <param name="color14">14th Color of the Palette</param>
    /// <param name="color15">15th Color of the Palette</param>
    /// <param name="color16">16th Color of the Palette</param>
    /// <remarks>
    /// No duplicate colors are allowed
    /// </remarks>
    public NexusColorPalette(
        in NexusColor color1,
        in NexusColor color2,
        in NexusColor color3,
        in NexusColor color4,
        in NexusColor color5,
        in NexusColor color6,
        in NexusColor color7,
        in NexusColor color8,
        in NexusColor color9,
        in NexusColor color10,
        in NexusColor color11,
        in NexusColor color12,
        in NexusColor color13,
        in NexusColor color14,
        in NexusColor color15,
        in NexusColor color16)
        : this([color1, color2, color3, color4, color5, color6, color7, color8, color9, color10, color11, color12, color13, color14, color15, color16]) { }

    /// <summary>
    /// Get the index of the specified color or <see cref="NexusColorIndex.Invalid"/> if not found
    /// </summary>
    /// <param name="color">The color to check for</param>
    /// <returns><see cref="NexusColorIndex"/></returns>
    public NexusColorIndex GetIndex(in NexusColor color)
        => NexusColorIndex.UnClamped(Colors.GetKey(color));

    /// <summary>
    /// Get the color at a specific index
    /// </summary>
    /// <param name="index">The index of the color [0-15]</param>
    /// <returns><see cref="NexusColor"/></returns>
    public NexusColor this[in NexusColorIndex index] => this[index.Index];

    /// <inheritdoc/>
    public IEnumerator<NexusColor> GetEnumerator() => Colors.Values.GetEnumerator();

    /// <inheritdoc/>
    public static bool operator ==(NexusColorPalette left, NexusColorPalette right) => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(NexusColorPalette left, NexusColorPalette right) => !(left == right);

    /// <inheritdoc/>
    public bool Equals(NexusColorPalette? other)
    {
        if (other is null) return false;

        for (int i = 0; i < Colors.Count; i++)
        {
            if (this[i] != other[i]) return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is NexusColorPalette colorPalette && Equals(colorPalette);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Colors);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal NexusColor this[in int index] => Colors[(ConsoleColor)index];
}