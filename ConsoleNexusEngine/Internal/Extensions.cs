namespace ConsoleNexusEngine.Internal;

internal static class Extensions
{
    public static bool IsInRange(in this NexusCoord coord, in NexusCoord start, in NexusCoord end)
        => coord.X >= start.X && coord.Y >= start.Y && coord.X <= end.X && coord.Y <= end.Y;

    public static bool IsInRange(in this NexusCoord coord, in NexusCoord start, in NexusSize range)
        => coord.X >= start.X && coord.Y >= start.Y && coord.X <= start.X + range.Width && coord.Y <= start.Y + range.Height;

    public static IEnumerable<int> GetKeys(this IReadOnlyList<NexusColor> colors, NexusColor color)
    {
        for (int i = 0; i < colors.Count; i++)
        {
            if (colors[i] == color) yield return i;
        }

        yield return -1;
    }
}