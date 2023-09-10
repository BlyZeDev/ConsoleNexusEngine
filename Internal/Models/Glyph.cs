namespace ConsoleNexusEngine.Internal.Models;

internal readonly struct Glyph
{
    public readonly char Value;
    public readonly int ForegroundIndex;
    public readonly int BackgroundIndex;

    public Glyph(char value, int foregroundIndex, int backgroundIndex)
    {
        Value = value;
        ForegroundIndex = foregroundIndex;
        BackgroundIndex = backgroundIndex;
    }
}