namespace ConsoleNexusEngine.Internal;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal static class StackAlloc
{
    private const int StackSize = 1024;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Allow<T>(int size) => StackSize >= size * Marshal.SizeOf<T>();
}