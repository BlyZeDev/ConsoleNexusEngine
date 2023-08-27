namespace ConsoleNexusEngine.Common;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

public readonly partial record struct Color
{
    /// <summary>
    /// [R=255,G=255,B=255]
    /// </summary>
    public static Color White => new(0xFFFFFF);

    /// <summary>
    /// [R=0,G=0,B=0]
    /// </summary>
    public static Color Black => new(0x000000);

    /// <summary>
    /// [R=255,G=0,B=0]
    /// </summary>
    public static Color Red => new(0xFF0000);

    /// <summary>
    /// [R=0,G=255,B=0]
    /// </summary>
    public static Color Green => new(0x00FF00);

    /// <summary>
    /// [R=0,G=0,B=255]
    /// </summary>
    public static Color Blue => new(0x0000FF);

    /// <summary>
    /// [R=255,G=0,B=255]
    /// </summary>
    public static Color Magenta => new(0xFF00FF);

    /// <summary>
    /// [R=255,G=255,B=0]
    /// </summary>
    public static Color Yellow => new(0xFFFF00);

    /// <summary>
    /// [R=0,G=255,B=255]
    /// </summary>
    public static Color Cyan => new(0x00FFFF);

    /// <summary>
    /// Parses the color from a HEX value
    /// </summary>
    /// <param name="hex">Supported formats are #ABCDEF and ABCDEF</param>
    /// <param name="provider">Just keep it <see langword="null"/></param>
    /// <returns><see cref="Color"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Color Parse(ReadOnlySpan<char> hex, IFormatProvider? provider = null)
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
    /// <param name="provider">Just keep it <see langword="null"/></param>
    /// <param name="result">The parsed <see cref="Color"/> or <see langword="null"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool TryParse(ReadOnlySpan<char> hex, IFormatProvider? provider, [MaybeNullWhen(false)] out Color result)
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
    /// <returns><see cref="Color"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Color Parse(string hex, IFormatProvider? provider = null)
        => Parse(hex.AsSpan());

    /// <summary>
    /// Tries to parse the color from a HEX value
    /// </summary>
    /// <param name="hex">Supported formats are #ABCDEF and ABCDEF</param>
    /// <param name="provider">Just keep it <see langword="null"/></param>
    /// <param name="result">The parsed <see cref="Color"/> or <see langword="null"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool TryParse([NotNullWhen(true)] string? hex, IFormatProvider? provider, [MaybeNullWhen(false)] out Color result)
        => TryParse(hex.AsSpan(), null, out result);

    private static uint FromRgb(in byte r, in byte g, in byte b)
        => ((uint)r << 16) | ((uint)g << 8) | b;
}