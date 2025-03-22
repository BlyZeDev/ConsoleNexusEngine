﻿namespace ConsoleNexusEngine.Internal;

using System.Runtime.CompilerServices;

internal static class IndexDimensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Get1D(in int x, in int y, in int width) => width * y + x;
}