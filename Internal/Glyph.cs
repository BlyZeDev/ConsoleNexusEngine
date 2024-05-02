namespace ConsoleNexusEngine.Internal;

internal readonly record struct Glyph(in char Value, in int ForegroundIndex, in int BackgroundIndex);