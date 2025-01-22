namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal static partial class Native
{
    [LibraryImport(WinRing32, EntryPoint = "InitializeOls")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool InitializeOls_32();

    [LibraryImport(WinRing32, EntryPoint = "DeinitializeOls")]
    public static partial void DeinitializeOls_32();

    [LibraryImport(WinRing32, EntryPoint = "Rdmsr")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool Rdmsr_32(uint index, out uint eax, out uint edx);

    [LibraryImport(WinRing32, EntryPoint = "Cpuid")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool Cpuid_32(uint index, out uint eax, out uint ebx, out uint ecx, out uint edx);

    [LibraryImport(WinRing64, EntryPoint = "InitializeOls")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool InitializeOls_64();

    [LibraryImport(WinRing64, EntryPoint = "DeinitializeOls")]
    public static partial void DeinitializeOls_64();

    [LibraryImport(WinRing64, EntryPoint = "Rdmsr")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool Rdmsr_64(uint index, out uint eax, out uint edx);

    [LibraryImport(WinRing64, EntryPoint = "Cpuid")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool Cpuid_64(uint index, out uint eax, out uint ebx, out uint ecx, out uint edx);
}