namespace ConsoleNexusEngine.Graphics;

using System.Collections.Immutable;

/// <summary>
/// Uses the RGB color space for image processing
/// </summary>
public sealed class NexusRgbProcessor : NexusImageProcessor
{
    private readonly ImmutableArray<NexusColor> _colors;

    /// <inheritdoc/>
    public NexusRgbProcessor(NexusColorPalette colorPalette) : base(colorPalette)
    {
        _colors = [.. _colorPalette];
    }

    /// <inheritdoc/>
    public override NexusColorIndex Process(in NexusColor targetColor)
    {
        var nearestColorIndex = 0;
        var minDistance = CalculateRgbDistance(targetColor, _colors[0]);

        for (int i = 1; i < _colors.Length; i++)
        {
            var distance = CalculateRgbDistance(targetColor, _colors[i]);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestColorIndex = i;
            }
        }

        return new NexusColorIndex(nearestColorIndex);
    }

    private static double CalculateRgbDistance(in NexusColor color1, in NexusColor color2)
    {
        var redDiff = color1.R - color2.R;
        var greenDiff = color1.G - color2.G;
        var blueDiff = color1.B - color2.B;

        return Math.Sqrt(Math.Pow(redDiff, 2) + Math.Pow(greenDiff, 2) + Math.Pow(blueDiff, 2));
    }
}