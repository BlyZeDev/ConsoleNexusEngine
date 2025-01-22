namespace ConsoleNexusEngine.Internal;

internal static class MathHelper
{
    public static int GetIndex(in int x, in int y, in int width) => width * y + x;
}