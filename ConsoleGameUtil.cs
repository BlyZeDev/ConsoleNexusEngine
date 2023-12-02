namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal;
using System;
using System.Security.Cryptography;

/// <summary>
/// Useful functions for a console game
/// </summary>
public sealed class ConsoleGameUtil
{
    internal ConsoleGameUtil() { }

    /// <summary>
    /// Generate a pseudo or strong random color
    /// </summary>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusColor"/></returns>
    public NexusColor GetRandomColor(bool pseudoRandom = true)
    {
        Span<byte> rgb = stackalloc byte[3];

        if (pseudoRandom) Random.Shared.NextBytes(rgb);
        else
            using (var rng = RandomNumberGenerator.Create())
                rng.GetNonZeroBytes(rgb);

        return new NexusColor(rgb[0], rgb[1], rgb[2]);
    }

    /// <summary>
    /// Generate a pseudo or strong random color from the palette
    /// </summary>
    /// <param name="colorPalette">The color palette to get a random color from</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusColor"/></returns>
    public NexusColor GetRandomColor(ColorPalette colorPalette, bool pseudoRandom = true)
        => colorPalette[pseudoRandom ? Random.Shared.Next(0, 16) : RandomNumberGenerator.GetInt32(0, 16)];

    /// <summary>
    /// Generate a pseudo or strong random coordinate
    /// </summary>
    /// <param name="maxWidth">Exclusive maximum width</param>
    /// <param name="maxHeight">Exclusive minimum height</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="Coord"/></returns>
    public Coord GetRandomCoord(int maxWidth, int maxHeight, bool pseudoRandom = true)
        => new(GetRandomNumber(maxWidth, pseudoRandom), GetRandomNumber(maxHeight, pseudoRandom));

    /// <summary>
    /// Generate a pseudo or strong random number
    /// </summary>
    /// <param name="minValue">Inclusive lower bound</param>
    /// <param name="maxValue">Exclusive upper bounds</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="int"/></returns>
    public int GetRandomNumber(int minValue = 0, int maxValue = int.MaxValue, bool pseudoRandom = true)
        => pseudoRandom ? Random.Shared.Next(minValue, maxValue) : RandomNumberGenerator.GetInt32(minValue, maxValue);

    /// <summary>
    /// Generate a pseudo or strong random number
    /// </summary>
    /// <param name="maxValue">Exclusive upper bounds</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="int"/></returns>
    public int GetRandomNumber(int maxValue, bool pseudoRandom = true)
        => GetRandomNumber(0, maxValue, pseudoRandom);
}