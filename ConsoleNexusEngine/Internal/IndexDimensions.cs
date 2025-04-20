namespace ConsoleNexusEngine.Internal;

using System.Runtime.CompilerServices;

internal static class IndexDimensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Get1D(int x, int y, int width) => width * y + x;
}