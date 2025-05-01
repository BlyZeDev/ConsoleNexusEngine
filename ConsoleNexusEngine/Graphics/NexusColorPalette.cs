namespace ConsoleNexusEngine.Graphics;

using System.Collections;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Represents a color palette with 16 colors for the console
/// </summary>
public abstract record NexusColorPalette : IEnumerable<NexusColor>
{
    /// <summary>
    /// The maximum amount of colors that can be in a color palette
    /// </summary>
    public const int MaxColorCount = 16;

    private readonly ImmutableArray<NexusColor> _colors;

    /// <summary>
    /// The colors of the color palette sorted by index
    /// </summary>
    protected ImmutableArray<NexusColor> Colors
    {
        get => _colors;
        init
        {
            ThrowIfPaletteInvalid(value);

            _colors = value;
        }
    }
    
    /// <summary>
    /// 1st Color of the Palette
    /// </summary>
    /// <remarks>
    /// This color is always the background color
    /// </remarks>
    public NexusColor Color1 => _colors[0];

    /// <summary>
    /// 2nd Color of the Palette
    /// </summary>
    public NexusColor Color2 => _colors[1];

    /// <summary>
    /// 3rd Color of the Palette
    /// </summary>
    public NexusColor Color3 => _colors[2];

    /// <summary>
    /// 4th Color of the Palette
    /// </summary>
    public NexusColor Color4 => _colors[3];

    /// <summary>
    /// 5th Color of the Palette
    /// </summary>
    public NexusColor Color5 => _colors[4];

    /// <summary>
    /// 6th Color of the Palette
    /// </summary>
    public NexusColor Color6 => _colors[5];

    /// <summary>
    /// 7th Color of the Palette
    /// </summary>
    public NexusColor Color7 => _colors[6];

    /// <summary>
    /// 8th Color of the Palette
    /// </summary>
    public NexusColor Color8 => _colors[7];

    /// <summary>
    /// 9th Color of the Palette
    /// </summary>
    public NexusColor Color9 => _colors[8];

    /// <summary>
    /// 10th Color of the Palette
    /// </summary>
    public NexusColor Color10 => _colors[9];

    /// <summary>
    /// 11th Color of the Palette
    /// </summary>
    public NexusColor Color11 => _colors[10];

    /// <summary>
    /// 12th Color of the Palette
    /// </summary>
    public NexusColor Color12 => _colors[11];

    /// <summary>
    /// 13th Color of the Palette
    /// </summary>
    public NexusColor Color13 => _colors[12];

    /// <summary>
    /// 14th Color of the Palette
    /// </summary>
    public NexusColor Color14 => _colors[13];

    /// <summary>
    /// 15th Color of the Palette
    /// </summary>
    public NexusColor Color15 => _colors[14];

    /// <summary>
    /// 16th Color of the Palette
    /// </summary>
    public NexusColor Color16 => _colors[15];

    /// <summary>
    /// This constructor does nothing
    /// </summary>
    protected NexusColorPalette() { }

    /// <summary>
    /// Get the first index of the specified color or <see langword="default"/> if not found
    /// </summary>
    /// <param name="color">The color to check for</param>
    /// <returns><see cref="NexusColorIndex"/></returns>
    public NexusColorIndex? GetIndex(in NexusColor color)
    {
        var index = GetKeys(color).First();

        return new NexusColorIndex(index == -1 ? default : index);
    }

    /// <summary>
    /// Get all indices of the specified color or <see cref="Enumerable.Empty{TResult}"/> if not found
    /// </summary>
    /// <param name="color">The color to check for</param>
    /// <returns><see cref="IEnumerable{NexusColorIndex}"/> of <see cref="NexusColorIndex"/></returns>
    public IEnumerable<NexusColorIndex> GetIndices(NexusColor color)
    {
        foreach (var index in GetKeys(color))
        {
            yield return new NexusColorIndex(index);
        }
    }

    /// <summary>
    /// Get the color at a specific index
    /// </summary>
    /// <param name="index">The index of the color [0-15]</param>
    /// <returns><see cref="NexusColor"/></returns>
    public NexusColor this[in NexusColorIndex index] => this[index.Value];

    /// <inheritdoc/>
    public IEnumerator<NexusColor> GetEnumerator() => _colors.AsEnumerable().GetEnumerator();

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_colors);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal NexusColor this[int index] => _colors[index];

    private IEnumerable<int> GetKeys(NexusColor color)
    {
        for (int i = 0; i < _colors.Length; i++)
        {
            if (_colors[i] == color) yield return i;
        }

        yield return -1;
    }

    private static void ThrowIfPaletteInvalid(in ImmutableArray<NexusColor> colors)
    {
        if (colors.IsDefault)
            throw new NullReferenceException($"{nameof(_colors)} was default");

        if (colors.Length != MaxColorCount)
            throw new InvalidOperationException($"{nameof(_colors)} must contain exactly 16 colors");
    }
}