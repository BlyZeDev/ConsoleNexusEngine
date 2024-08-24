﻿namespace ConsoleNexusEngine.Graphics;

using System.Collections;

/// <summary>
/// Represents a color palette with 16 unique colors for the console
/// </summary>
public abstract class NexusColorPalette : IEnumerable<NexusColor>
{
    /// <summary>
    /// The maximum amount of different colors that can be in a color palette
    /// </summary>
    public const int MaxColorCount = 16;

    /// <summary>
    /// The colors of the color palette sorted by index
    /// </summary>
    protected virtual IReadOnlyList<NexusColor> Colors { get; }
    
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
    public NexusColor Color2 => Colors[1];

    /// <summary>
    /// 3rd Color of the Palette
    /// </summary>
    public NexusColor Color3 => Colors[2];

    /// <summary>
    /// 4th Color of the Palette
    /// </summary>
    public NexusColor Color4 => Colors[3];

    /// <summary>
    /// 5th Color of the Palette
    /// </summary>
    public NexusColor Color5 => Colors[4];

    /// <summary>
    /// 6th Color of the Palette
    /// </summary>
    public NexusColor Color6 => Colors[5];

    /// <summary>
    /// 7th Color of the Palette
    /// </summary>
    public NexusColor Color7 => Colors[6];

    /// <summary>
    /// 8th Color of the Palette
    /// </summary>
    public NexusColor Color8 => Colors[7];

    /// <summary>
    /// 9th Color of the Palette
    /// </summary>
    public NexusColor Color9 => Colors[8];

    /// <summary>
    /// 10th Color of the Palette
    /// </summary>
    public NexusColor Color10 => Colors[9];

    /// <summary>
    /// 11th Color of the Palette
    /// </summary>
    public NexusColor Color11 => Colors[10];

    /// <summary>
    /// 12th Color of the Palette
    /// </summary>
    public NexusColor Color12 => Colors[11];

    /// <summary>
    /// 13th Color of the Palette
    /// </summary>
    public NexusColor Color13 => Colors[12];

    /// <summary>
    /// 14th Color of the Palette
    /// </summary>
    public NexusColor Color14 => Colors[13];

    /// <summary>
    /// 15th Color of the Palette
    /// </summary>
    public NexusColor Color15 => Colors[14];

    /// <summary>
    /// 16th Color of the Palette
    /// </summary>
    public NexusColor Color16 => Colors[15];

    /// <summary>
    /// Initializes a new <see cref="NexusColorPalette"/> with 16 colors
    /// </summary>
    /// <param name="colors">The colors of the color palette</param>
    /// <exception cref="InvalidOperationException"></exception>
    protected NexusColorPalette(IReadOnlyList<NexusColor>? colors = null)
    {
        if (colors is not null) Colors = colors;

        if (Colors is null)
            throw new NullReferenceException($"{nameof(Colors)} was null");
        
        if (Colors.Count is not MaxColorCount)
            throw new InvalidOperationException($"{nameof(Colors)} must contain exactly 16 unique colors");

        if (new HashSet<NexusColor>(Colors).Count is not MaxColorCount)
            throw new InvalidOperationException($"{nameof(Colors)} must contain exactly 16 unique colors");
    }

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
    public IEnumerator<NexusColor> GetEnumerator() => Colors.GetEnumerator();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is NexusColorPalette colorPalette && Equals(colorPalette);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Colors);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal NexusColor this[in int index] => Colors[index];
}