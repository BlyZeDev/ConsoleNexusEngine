namespace Snake;

using ConsoleNexusEngine.Graphics;
using System.Collections.Generic;

public sealed class SnakeColorPalette : NexusColorPalette
{
    private static readonly IReadOnlyList<NexusColor> _colors;

    static SnakeColorPalette()
    {
        _colors = [
            NexusColor.Black,
            NexusColor.Parse("#62AB46"), // Grass Background
            NexusColor.Parse("#9DC1C0"),
            NexusColor.Parse("#525B80"),
            NexusColor.Parse("#312139"),
            NexusColor.Parse("#120E1F"), // Fruit Color
            NexusColor.Parse("#284646"),
            NexusColor.Parse("#95533D"), // Border Color
            NexusColor.Parse("#6A2435"),
            NexusColor.Parse("#654147"),
            NexusColor.Parse("#FFF169"),
            NexusColor.Parse("#D7793F"), // Snake Body
            NexusColor.Parse("#AB3229"), // Snake Head
            NexusColor.Parse("#9E8F84"),
            NexusColor.Parse("#FFFACE"), // Point Counter
            NexusColor.Parse("#F68B69")];
    }

    protected override IReadOnlyList<NexusColor> Colors => _colors;
}