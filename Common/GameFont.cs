namespace ConsoleNexusEngine.Common;

using System;

/// <summary>
/// Represents a font for the console
/// </summary>
public sealed record GameFont
{
    /// <summary>
    /// The name of the font
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The size of the font in pixeln
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// The thickness of the font, 400 = Normal, 700 = Bold
    /// </summary>
    public int Weight { get; }

    /// <summary>
    /// Initializes a new game font
    /// </summary>
    /// <param name="name">The name of the font</param>
    /// <param name="size">The size of the font in pixeln</param>
    /// <param name="weight">The thickness of the font, 400 = Normal, 700 = Bold</param>
    public GameFont(string name, int size, int weight)
    {
        if (name.Length > 31)
            throw new ArgumentException("The font name can't be longer than 31", nameof(name));

        Name = name;
        Size = size;
        Weight = weight;
    }
}