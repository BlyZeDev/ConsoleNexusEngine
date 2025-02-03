namespace ConsoleNexusEngine;

using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

/// <summary>
/// Provides useful functions for <see cref="ConsoleGame"/>
/// </summary>
public sealed class ConsoleGameUtil
{
    private static readonly ReadOnlyMemory<NexusSpecialChar> _specialChars;
    private static readonly ReadOnlyMemory<NexusColorPalette> _colorPalettes;

    static ConsoleGameUtil()
    {
        _specialChars = Enum.GetValues<NexusSpecialChar>();

        var colorPalettes = new List<NexusColorPalette>();
        var type = typeof(NexusColorPalette);

        foreach (var cp in AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(type) && x.GetCustomAttribute<IgnoreColorPaletteAttribute>() is null))
        {
            colorPalettes.Add((NexusColorPalette)Activator.CreateInstance(cp)!);
        }

        _colorPalettes = new ReadOnlyMemory<NexusColorPalette>(colorPalettes.ToArray());
    }

    private readonly CmdConsole _console;
    private readonly ConsoleGameSettings _settings;

    internal ConsoleGameUtil(CmdConsole console, ConsoleGameSettings settings)
    {
        _console = console;
        _settings = settings;
    }

    /// <summary>
    /// <see langword="true"/> if <paramref name="coord"/> is in range of <paramref name="start"/> and <paramref name="end"/>
    /// </summary>
    /// <param name="coord">The coordinate to check for</param>
    /// <param name="start">The start coordinate</param>
    /// <param name="end">The end coordinate</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsInRange(in NexusCoord coord, in NexusCoord start, in NexusCoord end) => coord.IsInRange(start, end);

    /// <summary>
    /// <see langword="true"/> if <paramref name="coord"/> is in range of <paramref name="start"/> and <paramref name="range"/>
    /// </summary>
    /// <param name="coord">The coordinate to check for</param>
    /// <param name="start">The start coordinate</param>
    /// <param name="range">The range size</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsInRange(in NexusCoord coord, in NexusCoord start, in NexusSize range) => coord.IsInRange(start, range);

    /// <summary>
    /// Generate a pseudo or strong random color
    /// </summary>
    /// <param name="onlyColorPalette"><see langword="true"/> if only colors from the current <see cref="NexusColorPalette"/> should be included</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusColor"/></returns>
    public NexusColor GetRandomColor(in bool onlyColorPalette, in bool pseudoRandom = true)
    {
        if (onlyColorPalette) return _settings.ColorPalette[GetRandomNumber(NexusColorPalette.MaxColorCount, pseudoRandom)];

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
    /// <param name="inBufferArea"><see langword="false"/> if the coordinate should be in range of the buffer area</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusCoord"/></returns>
    public NexusCoord GetRandomCoord(in bool inBufferArea, in bool pseudoRandom = true)
        => inBufferArea ? GetRandomCoord(new NexusCoord(_console.Buffer.Width, _console.Buffer.Height), pseudoRandom) : new NexusCoord(GetRandomNumber(pseudoRandom), GetRandomNumber(pseudoRandom));

    /// <summary>
    /// Generate a pseudo or strong random coordinate
    /// </summary>
    /// <param name="maxCoord">The exclusive maximum coordinate</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusCoord"/></returns>
    public NexusCoord GetRandomCoord(in NexusCoord maxCoord, in bool pseudoRandom = true)
        => GetRandomCoord(NexusCoord.MinValue, maxCoord, pseudoRandom);

    /// <summary>
    /// Generate a pseudo or strong random coordinate
    /// </summary>
    /// <param name="minCoord">The inclusive minimum coordinate</param>
    /// <param name="maxCoord">The exclusive maximum coordinate</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusCoord"/></returns>
    public NexusCoord GetRandomCoord(in NexusCoord minCoord, in NexusCoord maxCoord, in bool pseudoRandom = true)
        => new(GetRandomNumber(minCoord.X, maxCoord.X, pseudoRandom), GetRandomNumber(minCoord.Y, maxCoord.Y, pseudoRandom));

    /// <summary>
    /// Generate a pseudo or strong random coordinate
    /// </summary>
    /// <param name="inBufferArea"><see langword="false"/> if the coordinate should be in range of the buffer area</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusSize"/></returns>
    public NexusSize GetRandomSize(in bool inBufferArea, in bool pseudoRandom = true)
        => inBufferArea ? GetRandomSize(new NexusSize(_console.Buffer.Width, _console.Buffer.Height), pseudoRandom) : new NexusSize(GetRandomNumber(pseudoRandom), GetRandomNumber(pseudoRandom));

    /// <summary>
    /// Generate a pseudo or strong random coordinate
    /// </summary>
    /// <param name="maxSize">The exclusive maximum coordinate</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusSize"/></returns>
    public NexusSize GetRandomSize(in NexusSize maxSize, in bool pseudoRandom = true)
        => GetRandomSize(NexusSize.MinValue, maxSize, pseudoRandom);

    /// <summary>
    /// Generate a pseudo or strong random coordinate
    /// </summary>
    /// <param name="minSize">The inclusive minimum coordinate</param>
    /// <param name="maxSize">The exclusive maximum coordinate</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusSize"/></returns>
    public NexusSize GetRandomSize(in NexusSize minSize, in NexusSize maxSize, in bool pseudoRandom = true)
        => new(GetRandomNumber(minSize.Width, maxSize.Width, pseudoRandom), GetRandomNumber(minSize.Height, maxSize.Height, pseudoRandom));

    /// <summary>
    /// Generate a pseudo or strong random number
    /// </summary>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="int"/></returns>
    public int GetRandomNumber(in bool pseudoRandom = true)
        => GetRandomNumber(0, int.MaxValue, pseudoRandom);

    /// <summary>
    /// Generate a pseudo or strong random number
    /// </summary>
    /// <param name="maxValue">Exclusive upper bounds</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="int"/></returns>
    public int GetRandomNumber(in int maxValue, in bool pseudoRandom = true)
        => GetRandomNumber(0, maxValue, pseudoRandom);

    /// <summary>
    /// Generate a pseudo or strong random number
    /// </summary>
    /// <param name="minValue">Inclusive lower bound</param>
    /// <param name="maxValue">Exclusive upper bounds</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="int"/></returns>
    public int GetRandomNumber(in int minValue, in int maxValue, in bool pseudoRandom = true)
        => pseudoRandom ? Random.Shared.Next(minValue, maxValue) : RandomNumberGenerator.GetInt32(minValue, maxValue);

    /// <summary>
    /// Generate a pseudo or strong random <see cref="NexusSpecialChar"/>
    /// </summary>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusSpecialChar"/></returns>
    public NexusSpecialChar GetRandomSpecialChar(in bool pseudoRandom = true)
        => _specialChars.Span[GetRandomNumber(_specialChars.Length, pseudoRandom)];

    /// <summary>
    /// Generate a pseudo or strong random character
    /// </summary>
    /// <param name="minValue">Inclusive lower bounds</param>
    /// <param name="maxValue">Exclusive upper bounds</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="char"/></returns>
    public char GetRandomChar(in char minValue, in char maxValue, in bool pseudoRandom = true)
        => (char)GetRandomNumber(minValue, maxValue, pseudoRandom);


    /// <summary>
    /// Generate a pseudo or strong random character
    /// </summary>
    /// <param name="maxValue">Exclusive upper bounds</param>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="char"/></returns>
    public char GetRandomChar(in char maxValue, in bool pseudoRandom = true)
        => GetRandomChar(char.MinValue, maxValue, pseudoRandom);

    /// <summary>
    /// Generate a pseudo or strong random character
    /// </summary>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="char"/></returns>
    public char GetRandomChar(in bool pseudoRandom = true)
        => GetRandomChar(char.MinValue, char.MaxValue, pseudoRandom);

    /// <summary>
    /// Generate a pseudo or strong random color palette
    /// </summary>
    /// <param name="pseudoRandom"><see langword="false"/> if it should be generated as a strong random</param>
    /// <returns><see cref="NexusColorPalette"/></returns>
    public NexusColorPalette GetRandomColorPalette(in bool pseudoRandom = true)
        => _colorPalettes.Span[GetRandomNumber(_colorPalettes.Length, pseudoRandom)];

    /// <summary>
    /// Displays a message box
    /// </summary>
    /// <param name="caption">The title of the message box</param>
    /// <param name="message">The title of the message box</param>
    /// <param name="alertIcon">The icon that should be displayed</param>
    public void ShowAlert(string caption, string message, in NexusAlertIcon alertIcon)
        => _console.MessageBox(caption, message, (uint)alertIcon);
}