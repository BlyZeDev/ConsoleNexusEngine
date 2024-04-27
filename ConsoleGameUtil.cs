namespace ConsoleNexusEngine;

using System.Security.Cryptography;

/// <summary>
/// Useful functions for <see cref="ConsoleGame"/>
/// </summary>
public sealed class ConsoleGameUtil
{
    private static readonly IReadOnlyList<NexusSpecialChar> _specialChars;

    static ConsoleGameUtil() => _specialChars = Enum.GetValues<NexusSpecialChar>();

    private readonly ConsoleGameSettings _settings;

    internal ConsoleGameUtil(ConsoleGameSettings settings) => _settings = settings;

    /// <summary>
    /// Generate a pseudo or strong random color
    /// </summary>
    /// <param name="onlyColorPalette"><see langword="true"/> if only colors from the current <see cref="NexusColorPalette"/> should be included</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusColor"/></returns>
    public NexusColor GetRandomColor(bool onlyColorPalette, bool pseudoRandom = true)
    {
        if (onlyColorPalette) return _settings.ColorPalette[GetRandomNumber(16, pseudoRandom)];

        Span<byte> rgb = stackalloc byte[3];

        if (pseudoRandom) Random.Shared.NextBytes(rgb);
        else
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(rgb);
            }
        }

        return new NexusColor(rgb[0], rgb[1], rgb[2]);
    }

    /// <summary>
    /// Generate a pseudo or strong random coordinate
    /// </summary>
    /// <param name="maxWidth">Exclusive maximum width</param>
    /// <param name="maxHeight">Exclusive maximum height</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusCoord"/></returns>
    public NexusCoord GetRandomCoord(int maxWidth, int maxHeight, bool pseudoRandom = true)
        => new(GetRandomNumber(maxWidth, pseudoRandom), GetRandomNumber(maxHeight, pseudoRandom));

    /// <summary>
    /// Generate a pseudo or strong random number
    /// </summary>
    /// <param name="maxValue">Exclusive upper bounds</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="int"/></returns>
    public int GetRandomNumber(int maxValue, bool pseudoRandom = true)
        => GetRandomNumber(0, maxValue, pseudoRandom);

    /// <summary>
    /// Generate a pseudo or strong random number
    /// </summary>
    /// <param name="minValue">Inclusive lower bound</param>
    /// <param name="maxValue">Exclusive upper bounds</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="int"/></returns>
    public int GetRandomNumber(int minValue, int maxValue, bool pseudoRandom = true)
        => pseudoRandom ? Random.Shared.Next(minValue, maxValue) : RandomNumberGenerator.GetInt32(minValue, maxValue);

    /// <summary>
    /// Generate a pseudo or strong random <see cref="NexusSpecialChar"/>
    /// </summary>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusSpecialChar"/></returns>
    public NexusSpecialChar GetRandomSpecialChar(bool pseudoRandom = true)
        => _specialChars[GetRandomNumber(_specialChars.Count, pseudoRandom)];

    /// <summary>
    /// Generate a pseudo or strong random character
    /// </summary>
    /// <param name="minValue">Inclusive lower bounds</param>
    /// <param name="maxValue">Exclusive upper bounds</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="char"/></returns>
    public char GetRandomChar(char minValue, char maxValue, bool pseudoRandom = true)
        => (char)GetRandomNumber(minValue, maxValue, pseudoRandom);


    /// <summary>
    /// Generate a pseudo or strong random character
    /// </summary>
    /// <param name="maxValue">Exclusive upper bounds</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="char"/></returns>
    public char GetRandomChar(char maxValue, bool pseudoRandom = true)
        => GetRandomChar(char.MinValue, maxValue, pseudoRandom);

    /// <summary>
    /// Generate a pseudo or strong random character
    /// </summary>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="char"/></returns>
    public char GetRandomChar(bool pseudoRandom = true)
        => GetRandomChar(char.MinValue, char.MaxValue, pseudoRandom);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusColorPalette"/></returns>
    public NexusColorPalette GetRandomColorPalette(bool pseudoRandom = true)
        => NexusColorPalette._presets[GetRandomNumber(NexusColorPalette._presets.Length, pseudoRandom)];
}