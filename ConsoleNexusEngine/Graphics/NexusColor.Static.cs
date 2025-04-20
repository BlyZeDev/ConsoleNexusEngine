namespace ConsoleNexusEngine.Graphics;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

public readonly partial record struct NexusColor
{
    /// <summary>
    /// [R=255,G=255,B=255]
    /// </summary>
    public static NexusColor White => new NexusColor(0xFFFFFF);

    /// <summary>
    /// [R=0,G=0,B=0]
    /// </summary>
    public static NexusColor Black => new NexusColor(0x000000);

    /// <summary>
    /// [R=255,G=0,B=0]
    /// </summary>
    public static NexusColor Red => new NexusColor(0xFF0000);

    /// <summary>
    /// [R=0,G=255,B=0]
    /// </summary>
    public static NexusColor Green => new NexusColor(0x00FF00);

    /// <summary>
    /// [R=0,G=0,B=255]
    /// </summary>
    public static NexusColor Blue => new NexusColor(0x0000FF);

    /// <summary>
    /// [R=255,G=0,B=255]
    /// </summary>
    public static NexusColor Magenta => new NexusColor(0xFF00FF);

    /// <summary>
    /// [R=255,G=255,B=0]
    /// </summary>
    public static NexusColor Yellow => new NexusColor(0x00FFFF);

    /// <summary>
    /// [R=0,G=255,B=255]
    /// </summary>
    public static NexusColor Cyan => new NexusColor(0xFFFF00);

    /// <summary>
    /// Parses the color from a HEX value
    /// </summary>
    /// <param name="hex">Supported formats are #ABCDEF and ABCDEF</param>
    /// <param name="provider">This parameter is ignored</param>
    /// <returns><see cref="NexusColor"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public static NexusColor Parse(ReadOnlySpan<char> hex, IFormatProvider? provider = null)
    {
        if (!(hex.Length is 6 or 7))
            throw new ArgumentException("HEX value must be 6 or 7 characters", nameof(hex));

        hex = hex[0] == '#' ? hex[1..] : hex;

        return new(uint.Parse(hex, NumberStyles.HexNumber));
    }

    /// <summary>
    /// Tries to parse the color from a HEX value
    /// </summary>
    /// <param name="hex">Supported formats are #ABCDEF and ABCDEF</param>
    /// <param name="provider">This parameter is ignored</param>
    /// <param name="result">The parsed <see cref="NexusColor"/> or <see langword="null"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool TryParse(ReadOnlySpan<char> hex, IFormatProvider? provider, [MaybeNullWhen(false)] out NexusColor result)
    {
        try
        {
            result = Parse(hex);
            return true;
        }
        catch (Exception)
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Parses the color from a HEX value
    /// </summary>
    /// <param name="hex">Supported formats are #ABCDEF and ABCDEF</param>
    /// <param name="provider">Just keep it <see langword="null"/></param>
    /// <returns><see cref="NexusColor"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public static NexusColor Parse(string hex, IFormatProvider? provider = null)
        => Parse(hex.AsSpan());

    /// <summary>
    /// Tries to parse the color from a HEX value
    /// </summary>
    /// <param name="hex">Supported formats are #ABCDEF and ABCDEF</param>
    /// <param name="provider">Just keep it <see langword="null"/></param>
    /// <param name="result">The parsed <see cref="NexusColor"/> or <see langword="null"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool TryParse([NotNullWhen(true)] string? hex, IFormatProvider? provider, [MaybeNullWhen(false)] out NexusColor result)
        => TryParse(hex.AsSpan(), null, out result);

    private static uint FromRgb(byte r, byte g, byte b)
        => ((uint)r << 16) | ((uint)g << 8) | b;
}