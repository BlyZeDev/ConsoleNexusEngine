namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using System;
using System.Security.Cryptography;

/// <summary>
/// Useful functions for a console game
/// </summary>
public sealed class ConsoleGameUtil
{
    internal ConsoleGameUtil() { }

    public NexusColor GetRandomColor(bool pseudoRandom = true)
    {
        Span<byte> rgb = stackalloc byte[3];

        if (pseudoRandom) Random.Shared.NextBytes(rgb);
        else
            using (var rng = RandomNumberGenerator.Create())
                rng.GetNonZeroBytes(rgb);

        return new NexusColor(rgb[0], rgb[1], rgb[2]);
    }

    public NexusColor GetRandomColor(ColorPalette colorPalette, bool pseudoRandom = true)
        => colorPalette[pseudoRandom ? Random.Shared.Next(0, 16) : RandomNumberGenerator.GetInt32(0, 16)];

}